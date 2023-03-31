// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;
using FluentAssertions;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.VariableRecurringPayments.
    DomesticVrp;

public class DomesticVrpSubtest
{
    public static ISet<DomesticVrpSubtestEnum> DomesticVrpFunctionalSubtestsSupported(BankProfile bankProfile) =>
        bankProfile.VariableRecurringPaymentsApi is null
            ? new HashSet<DomesticVrpSubtestEnum>() // empty set
            : DomesticVrpSubtestHelper.AllDomesticVrpSubtests;

    public static async Task RunTest(
        DomesticVrpSubtestEnum subtestEnum,
        BankProfile bankProfile,
        Guid bankId,
        Guid bankRegistrationId,
        VariableRecurringPaymentsApiSettings variableRecurringPaymentsApiSettings,
        IRequestBuilder requestBuilderIn,
        Func<IRequestBuilderContainer> requestBuilderGenerator,
        string testNameUnique,
        string modifiedBy,
        FilePathBuilder configFluentRequestLogging,
        FilePathBuilder vrpFluentRequestLogging,
        ConsentAuth? consentAuth,
        List<BankUser> bankUserList)
    {
        bool subtestSkipped = subtestEnum switch
        {
            DomesticVrpSubtestEnum.VrpWithDebtorAccountSpecifiedByPisp => true,
            DomesticVrpSubtestEnum.VrpWithDebtorAccountSpecifiedDuringConsentAuthorisation => false,
            DomesticVrpSubtestEnum
                    .VrpWithDebtorAccountSpecifiedDuringConsentAuthorisationAndCreditorAccountSpecifiedDuringPaymentInitiation
                => true,
            _ => throw new ArgumentException(
                $"{nameof(subtestEnum)} is not valid {nameof(DomesticVrpSubtestEnum)} or needs to be added to this switch statement.")
        };
        if (subtestSkipped)
        {
            return;
        }

        // For now, we just use first bank user in list. Maybe later we can use different users for
        // different sub-tests.
        BankUser bankUser = bankUserList[0];

        IRequestBuilder requestBuilder = requestBuilderIn;

        // Basic request object for domestic payment consent
        var domesticVrpConsentRequest = new DomesticVrpConsentRequest
        {
            BankRegistrationId = default, // substitute logging placeholder
            TemplateRequest = new DomesticVrpTemplateRequest
            {
                Type = DomesticVrpSubtestHelper.GetDomesticVrpConsentTemplateType(subtestEnum),
                Parameters = new DomesticVrpTemplateParameters
                {
                    InstructionIdentification = "placeholder", // substitute logging placeholder
                    EndToEndIdentification = "placeholder" // substitute logging placeholder
                }
            }
        };

        domesticVrpConsentRequest.ExternalApiRequest =
            DomesticVrpConsentPublicMethods.ResolveExternalApiRequest(
                domesticVrpConsentRequest.ExternalApiRequest,
                domesticVrpConsentRequest.TemplateRequest,
                bankProfile); // Resolve for fuller logging

        await vrpFluentRequestLogging
            .AppendToPath("domesticVrpConsent")
            .AppendToPath("postRequest")
            .WriteFile(domesticVrpConsentRequest);

        // Basic request object for domestic payment
        requestBuilder.Utility.Map(
            domesticVrpConsentRequest.ExternalApiRequest,
            out VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest obDomesticVrpRequest);
        var domesticVrpRequest =
            new DomesticVrpRequest
            {
                ExternalApiRequest = obDomesticVrpRequest
            };

        // POST domestic payment consent
        DomesticVrpAccountIndexPair domesticVrpAccountIndexPair = bankUser.DomesticVrpAccountIndexPairs[0];
        Account creditorAccount = bankUser.Accounts[domesticVrpAccountIndexPair.Dest];
        domesticVrpConsentRequest.ExternalApiRequest.Data.Initiation.CreditorAccount.SchemeName =
            creditorAccount.SchemeName;
        domesticVrpConsentRequest.ExternalApiRequest.Data.Initiation.CreditorAccount.Identification =
            creditorAccount.Identification;
        domesticVrpConsentRequest.ExternalApiRequest.Data.Initiation.CreditorAccount.Name =
            creditorAccount.Name;
        domesticVrpConsentRequest.ExternalApiRequest.Data.Initiation.CreditorAccount
                .SecondaryIdentification =
            creditorAccount.SecondaryIdentification;
        domesticVrpConsentRequest.BankRegistrationId = bankRegistrationId;
        domesticVrpConsentRequest.Reference = testNameUnique;
        DomesticVrpConsentCreateResponse domesticVrpConsentResp =
            await requestBuilder.VariableRecurringPayments.DomesticVrpConsents
                .CreateAsync(domesticVrpConsentRequest);

        // Checks
        domesticVrpConsentResp.Should().NotBeNull();
        domesticVrpConsentResp.Warnings.Should().BeNull();
        domesticVrpConsentResp.ExternalApiResponse.Should().NotBeNull();
        Guid domesticVrpConsentId = domesticVrpConsentResp.Id;

        // GET domestic VRP consent
        DomesticVrpConsentCreateResponse domesticVrpConsentResp2 =
            await requestBuilder.VariableRecurringPayments.DomesticVrpConsents
                .ReadAsync(domesticVrpConsentId);

        // Checks
        domesticVrpConsentResp2.Should().NotBeNull();
        domesticVrpConsentResp2.Warnings.Should().BeNull();
        domesticVrpConsentResp2.ExternalApiResponse.Should().NotBeNull();

        // POST auth context
        var authContextRequest = new DomesticVrpConsentAuthContext
        {
            DomesticVrpConsentId = domesticVrpConsentId,
            Reference = testNameUnique
        };
        DomesticVrpConsentAuthContextCreateResponse authContextResponse =
            await requestBuilder.VariableRecurringPayments
                .DomesticVrpConsents
                .AuthContexts
                .CreateLocalAsync(authContextRequest);

        // Checks
        authContextResponse.Should().NotBeNull();
        authContextResponse.Warnings.Should().BeNull();
        authContextResponse.AuthUrl.Should().NotBeNull();
        Guid authContextId = authContextResponse.Id;
        string authUrl = authContextResponse.AuthUrl!;

        // GET auth context
        DomesticVrpConsentAuthContextReadResponse authContextResponse2 =
            await requestBuilder.VariableRecurringPayments.DomesticVrpConsents
                .AuthContexts
                .ReadLocalAsync(authContextId);
        // Checks
        authContextResponse2.Should().NotBeNull();
        authContextResponse2.Warnings.Should().BeNull();

        // Consent authorisation
        if (consentAuth is not null)
        {
            async Task<bool> AuthIsComplete()
            {
                DomesticVrpConsentCreateResponse consentResponse =
                    await requestBuilder
                        .VariableRecurringPayments
                        .DomesticVrpConsents
                        .ReadAsync(
                            domesticVrpConsentId,
                            modifiedBy,
                            false);
                return consentResponse.Created < consentResponse.AuthContextModified;
            }

            // Authorise consent in UI via Playwright
            await consentAuth.AuthoriseAsync(
                authUrl,
                bankProfile,
                ConsentVariety.DomesticVrpConsent,
                bankUser,
                AuthIsComplete);

            // Refresh scope to ensure user token acquired following consent is available
            using IRequestBuilderContainer scopedRequestBuilderNew = requestBuilderGenerator();
            IRequestBuilder requestBuilderNew = scopedRequestBuilderNew.RequestBuilder;

            if (variableRecurringPaymentsApiSettings.UseConsentGetFundsConfirmationEndpoint)
            {
                // GET consent funds confirmation
                DomesticVrpConsentReadFundsConfirmationResponse domesticPaymentConsentResp4 =
                    await requestBuilderNew.VariableRecurringPayments.DomesticVrpConsents
                        .ReadFundsConfirmationAsync(domesticVrpConsentId);

                // Checks
                domesticPaymentConsentResp4.Should().NotBeNull();
                domesticPaymentConsentResp4.Warnings.Should().BeNull();
                domesticPaymentConsentResp4.ExternalApiResponse.Should().NotBeNull();
            }

            // POST domestic payment
            await vrpFluentRequestLogging
                .AppendToPath("domesticVrp")
                .AppendToPath("postRequest")
                .WriteFile(domesticVrpRequest);
            domesticVrpRequest.Reference = testNameUnique;
            DomesticVrpResponse domesticVrpResp =
                await requestBuilderNew.VariableRecurringPayments.DomesticVrps
                    .CreateAsync(domesticVrpRequest, domesticVrpConsentId);

            // Checks
            domesticVrpResp.Should().NotBeNull();
            domesticVrpResp.Warnings.Should().BeNull();
            domesticVrpResp.ExternalApiResponse.Should().NotBeNull();
            string domesticVrpExternalId = domesticVrpResp.ExternalApiResponse.Data.DomesticVRPId;

            // GET domestic payment
            DomesticVrpResponse domesticVrpResp2 =
                await requestBuilderNew.VariableRecurringPayments.DomesticVrps
                    .ReadAsync(domesticVrpExternalId, domesticVrpConsentId);

            // Checks
            domesticVrpResp2.Should().NotBeNull();
            domesticVrpResp2.Warnings.Should().BeNull();
            domesticVrpResp2.ExternalApiResponse.Should().NotBeNull();

            // DELETE domestic payment consent
            var includeExternalApiOperation = true;
            ObjectDeleteResponse domesticVrpConsentResp3 = await requestBuilderNew.VariableRecurringPayments
                .DomesticVrpConsents
                .DeleteAsync(domesticVrpConsentId, modifiedBy, includeExternalApiOperation);

            // Checks
            domesticVrpConsentResp3.Should().NotBeNull();
            domesticVrpConsentResp3.Warnings.Should().BeNull();
        }
    }
}
