// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.RequestObjects.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.RequestObjects.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FluentAssertions;
using DomesticVrpConsentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrpConsent;
using DomesticVrpRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrp;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.VariableRecurringPayments.
    DomesticVrp
{
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

            // Create VariableRecurringPaymentsApi
            VariableRecurringPaymentsApiRequest variableRecurringPaymentsApiRequest =
                bankProfile.GetVariableRecurringPaymentsApiRequest(Guid.Empty);
            await configFluentRequestLogging
                .AppendToPath("variableRecurringPaymentsApi")
                .AppendToPath("postRequest")
                .WriteFile(variableRecurringPaymentsApiRequest);
            variableRecurringPaymentsApiRequest.Reference = testNameUnique;
            variableRecurringPaymentsApiRequest.BankId = bankId;
            IFluentResponse<VariableRecurringPaymentsApiResponse> variableRecurringPaymentsApiResponse =
                await requestBuilder
                    .BankConfiguration
                    .VariableRecurringPaymentsApis
                    .CreateLocalAsync(variableRecurringPaymentsApiRequest);
            variableRecurringPaymentsApiResponse.Should().NotBeNull();
            variableRecurringPaymentsApiResponse.Messages.Should().BeEmpty();
            variableRecurringPaymentsApiResponse.Data.Should().NotBeNull();
            Guid variableRecurringPaymentsApiId = variableRecurringPaymentsApiResponse.Data!.Id;

            // Basic request object for domestic payment consent
            DomesticVrpConsentRequest domesticVrpConsentRequest =
                bankProfile.DomesticVrpConsentRequest(
                    Guid.Empty,
                    Guid.Empty,
                    DomesticVrpSubtestHelper.DomesticVrpType(subtestEnum));
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
            domesticVrpConsentRequest.VariableRecurringPaymentsApiId = variableRecurringPaymentsApiId;
            domesticVrpConsentRequest.Reference = testNameUnique;
            IFluentResponse<DomesticVrpConsentReadResponse> domesticVrpConsentResp =
                await requestBuilder.VariableRecurringPayments.DomesticVrpConsents
                    .CreateAsync(domesticVrpConsentRequest);

            // Checks
            domesticVrpConsentResp.Should().NotBeNull();
            domesticVrpConsentResp.Messages.Should().BeEmpty();
            domesticVrpConsentResp.Data.Should().NotBeNull();
            Guid domesticVrpConsentId = domesticVrpConsentResp.Data!.Id;

            // GET domestic payment consent
            IFluentResponse<DomesticVrpConsentReadResponse> domesticVrpConsentResp2 =
                await requestBuilder.VariableRecurringPayments.DomesticVrpConsents
                    .ReadAsync(domesticVrpConsentId);

            // Checks
            domesticVrpConsentResp2.Should().NotBeNull();
            domesticVrpConsentResp2.Messages.Should().BeEmpty();
            domesticVrpConsentResp2.Data.Should().NotBeNull();

            // POST auth context
            var authContextRequest = new DomesticVrpConsentAuthContext
            {
                DomesticVrpConsentId = domesticVrpConsentId,
                Reference = testNameUnique
            };
            IFluentResponse<DomesticVrpConsentAuthContextCreateLocalResponse> authContextResponse =
                await requestBuilder.VariableRecurringPayments
                    .DomesticVrpConsents
                    .AuthContexts
                    .CreateLocalAsync(authContextRequest);

            // Checks
            authContextResponse.Should().NotBeNull();
            authContextResponse.Messages.Should().BeEmpty();
            authContextResponse.Data.Should().NotBeNull();
            authContextResponse.Data!.AuthUrl.Should().NotBeNull();
            Guid authContextId = authContextResponse.Data!.Id;
            string authUrl = authContextResponse.Data!.AuthUrl!;

            // GET auth context
            IFluentResponse<DomesticVrpConsentAuthContextReadLocalResponse> authContextResponse2 =
                await requestBuilder.VariableRecurringPayments.DomesticVrpConsents
                    .AuthContexts
                    .ReadLocalAsync(authContextId);
            // Checks
            authContextResponse2.Should().NotBeNull();
            authContextResponse2.Messages.Should().BeEmpty();
            authContextResponse2.Data.Should().NotBeNull();

            // Consent authorisation
            if (consentAuth is not null)
            {
                // Authorise consent in UI via Playwright
                await consentAuth.AuthoriseAsync(
                    authUrl,
                    bankProfile.BankProfileEnum,
                    ConsentVariety.DomesticVrpConsent,
                    bankUser);

                // Refresh scope to ensure user token acquired following consent is available
                using IRequestBuilderContainer scopedRequestBuilderNew = requestBuilderGenerator();
                IRequestBuilder requestBuilderNew = scopedRequestBuilderNew.RequestBuilder;

                if (variableRecurringPaymentsApiSettings.UseConsentGetFundsConfirmationEndpoint)
                {
                    // GET consent funds confirmation
                    IFluentResponse<DomesticVrpConsentReadFundsConfirmationResponse> domesticPaymentConsentResp4 =
                        await requestBuilderNew.VariableRecurringPayments.DomesticVrpConsents
                            .ReadFundsConfirmationAsync(domesticVrpConsentId);

                    // Checks
                    domesticPaymentConsentResp4.Should().NotBeNull();
                    domesticPaymentConsentResp4.Messages.Should().BeEmpty();
                    domesticPaymentConsentResp4.Data.Should().NotBeNull();
                }

                // POST domestic payment
                await vrpFluentRequestLogging
                    .AppendToPath("domesticVrp")
                    .AppendToPath("postRequest")
                    .WriteFile(domesticVrpRequest);
                domesticVrpRequest.Reference = testNameUnique;
                IFluentResponse<DomesticVrpResponse> domesticVrpResp =
                    await requestBuilderNew.VariableRecurringPayments.DomesticVrps
                        .CreateAsync(domesticVrpRequest, domesticVrpConsentId);

                // Checks
                domesticVrpResp.Should().NotBeNull();
                domesticVrpResp.Messages.Should().BeEmpty();
                domesticVrpResp.Data.Should().NotBeNull();
                string domesticVrpExternalId = domesticVrpResp.Data!.ExternalApiResponse.Data.DomesticVRPId;

                // GET domestic payment
                IFluentResponse<DomesticVrpResponse> domesticVrpResp2 =
                    await requestBuilderNew.VariableRecurringPayments.DomesticVrps
                        .ReadAsync(domesticVrpExternalId, domesticVrpConsentId);

                // Checks
                domesticVrpResp2.Should().NotBeNull();
                domesticVrpResp2.Messages.Should().BeEmpty();
                domesticVrpResp2.Data.Should().NotBeNull();

                // DELETE domestic payment consent
                IFluentResponse domesticVrpConsentResp3 = await requestBuilderNew.VariableRecurringPayments
                    .DomesticVrpConsents
                    .DeleteAsync(domesticVrpConsentId);

                // Checks
                domesticVrpConsentResp3.Should().NotBeNull();
                domesticVrpConsentResp3.Messages.Should().BeEmpty();
            }

            // DELETE API object
            IFluentResponse apiResponse = await requestBuilder
                .BankConfiguration
                .VariableRecurringPaymentsApis
                .DeleteLocalAsync(variableRecurringPaymentsApiId);

            // Checks
            apiResponse.Should().NotBeNull();
            apiResponse.Messages.Should().BeEmpty();
        }
    }
}
