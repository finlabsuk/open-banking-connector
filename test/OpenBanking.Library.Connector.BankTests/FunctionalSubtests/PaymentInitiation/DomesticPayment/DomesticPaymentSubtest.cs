// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using FluentAssertions;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.PaymentInitiation.DomesticPayment
{
    public class DomesticPaymentSubtest
    {
        public static ISet<DomesticPaymentSubtestEnum> DomesticPaymentFunctionalSubtestsSupported(
            BankProfile bankProfile) =>
            bankProfile.PaymentInitiationApi is null
                ? new HashSet<DomesticPaymentSubtestEnum>()
                : DomesticPaymentSubtestHelper.AllDomesticPaymentSubtests;

        public static async Task RunTest(
            DomesticPaymentSubtestEnum subtestEnum,
            BankProfile bankProfile,
            Guid bankId,
            Guid bankRegistrationId,
            PaymentInitiationApiSettings paymentInitiationApiSettings,
            IRequestBuilder requestBuilderIn,
            Func<IRequestBuilderContainer> requestBuilderGenerator,
            string testNameUnique,
            string modifiedBy,
            FilePathBuilder configFluentRequestLogging,
            FilePathBuilder pispFluentRequestLogging,
            ConsentAuth? consentAuth,
            List<BankUser> bankUserList,
            IApiClient apiClient)
        {
            bool subtestSkipped = subtestEnum switch
            {
                DomesticPaymentSubtestEnum.PersonToPersonSubtest =>
                    true,
                DomesticPaymentSubtestEnum.PersonToMerchantSubtest => false,
                _ => throw new ArgumentException(
                    $"{nameof(subtestEnum)} is not valid {nameof(DomesticPaymentSubtestEnum)} or needs to be added to this switch statement.")
            };
            if (subtestSkipped)
            {
                return;
            }

            // For now, we just use first bank user in list. Maybe later we can use different users for
            // different sub-tests.
            BankUser bankUser = bankUserList[0];

            IRequestBuilder requestBuilder = requestBuilderIn;

            // Create PaymentInitiationApi
            var paymentInitiationApiRequest =
                new PaymentInitiationApiRequest
                {
                    BankProfile = bankProfile.BankProfileEnum,
                    BankId = Guid.Empty,
                    ApiVersion =
                        bankProfile.PaymentInitiationApi?.PaymentInitiationApiVersion ??
                        throw new InvalidOperationException("No PaymentInitiationApi specified in bank profile."),
                    BaseUrl = bankProfile.PaymentInitiationApi.BaseUrl
                };
            await configFluentRequestLogging
                .AppendToPath("paymentInitiationApi")
                .AppendToPath("postRequest")
                .WriteFile(paymentInitiationApiRequest);
            paymentInitiationApiRequest.BankId = bankId;
            paymentInitiationApiRequest.Reference = testNameUnique;
            paymentInitiationApiRequest.CreatedBy = modifiedBy;
            PaymentInitiationApiResponse paymentInitiationApiResponse =
                await requestBuilder
                    .BankConfiguration
                    .PaymentInitiationApis
                    .CreateLocalAsync(paymentInitiationApiRequest);
            paymentInitiationApiResponse.Should().NotBeNull();
            paymentInitiationApiResponse.Warnings.Should().BeNull();
            Guid paymentInitiationApiId = paymentInitiationApiResponse.Id;

            // Read PaymentInitiationApi
            PaymentInitiationApiResponse paymentInitiationApiReadResponse =
                await requestBuilder
                    .BankConfiguration
                    .PaymentInitiationApis
                    .ReadLocalAsync(paymentInitiationApiId);

            // Checks
            paymentInitiationApiReadResponse.Should().NotBeNull();
            paymentInitiationApiReadResponse.Warnings.Should().BeNull();

            // Create DomesticPaymentConsent
            var domesticPaymentConsentRequest = new DomesticPaymentConsentRequest
            {
                BankProfile = bankProfile.BankProfileEnum,
                BankRegistrationId = default, // substitute logging placeholder
                PaymentInitiationApiId = default, // substitute logging placeholder
                TemplateRequest = new DomesticPaymentTemplateRequest
                {
                    Type = DomesticPaymentSubtestHelper
                        .GetDomesticPaymentTemplateType(subtestEnum),
                    Parameters = new DomesticPaymentTemplateParameters
                    {
                        InstructionIdentification = "placeholder", // substitute logging placeholder
                        EndToEndIdentification = "placeholder" // substitute logging placeholder
                    }
                }
            };

            domesticPaymentConsentRequest.ExternalApiRequest =
                DomesticPaymentConsentPublicMethods.ResolveExternalApiRequest(
                    domesticPaymentConsentRequest.ExternalApiRequest,
                    domesticPaymentConsentRequest.TemplateRequest,
                    bankProfile); // Resolve for fuller logging

            await pispFluentRequestLogging
                .AppendToPath("domesticPaymentConsent")
                .AppendToPath("postRequest")
                .WriteFile(domesticPaymentConsentRequest);

            // Basic request object for domestic payment
            requestBuilder.Utility.Map(
                domesticPaymentConsentRequest.ExternalApiRequest,
                out PaymentInitiationModelsPublic.OBWriteDomestic2 obWriteDomestic);
            var domesticPaymentRequest =
                new DomesticPaymentRequest
                {
                    ExternalApiRequest = obWriteDomestic
                };

            domesticPaymentConsentRequest.BankRegistrationId = bankRegistrationId; // remove logging placeholder
            domesticPaymentConsentRequest.PaymentInitiationApiId = paymentInitiationApiId; // remove logging placeholder
            domesticPaymentConsentRequest.ExternalApiRequest.Data.Initiation.InstructionIdentification =
                Guid.NewGuid().ToString("N"); // remove logging placeholder
            domesticPaymentConsentRequest.ExternalApiRequest.Data.Initiation.EndToEndIdentification =
                Guid.NewGuid().ToString("N"); // remove logging placeholder
            domesticPaymentConsentRequest.Reference = testNameUnique;
            domesticPaymentConsentRequest.CreatedBy = modifiedBy;
            DomesticPaymentConsentCreateResponse domesticPaymentConsentResp =
                await requestBuilder.PaymentInitiation.DomesticPaymentConsents
                    .CreateAsync(domesticPaymentConsentRequest);

            // Checks
            domesticPaymentConsentResp.Should().NotBeNull();
            domesticPaymentConsentResp.Warnings.Should().BeNull();
            domesticPaymentConsentResp.ExternalApiResponse.Should().NotBeNull();
            Guid domesticPaymentConsentId = domesticPaymentConsentResp.Id;

            // GET domestic payment consent
            DomesticPaymentConsentReadResponse domesticPaymentConsentResp2 =
                await requestBuilder.PaymentInitiation.DomesticPaymentConsents
                    .ReadAsync(domesticPaymentConsentId);

            // Checks
            domesticPaymentConsentResp2.Should().NotBeNull();
            domesticPaymentConsentResp2.Warnings.Should().BeNull();
            domesticPaymentConsentResp2.ExternalApiResponse.Should().NotBeNull();

            // POST auth context
            var authContextRequest = new DomesticPaymentConsentAuthContext
            {
                DomesticPaymentConsentId = domesticPaymentConsentId,
                Reference = testNameUnique,
                CreatedBy = modifiedBy
            };
            DomesticPaymentConsentAuthContextCreateResponse authContextResponse =
                await requestBuilder.PaymentInitiation
                    .DomesticPaymentConsents
                    .AuthContexts
                    .CreateLocalAsync(authContextRequest);

            // Checks
            authContextResponse.Should().NotBeNull();
            authContextResponse.Warnings.Should().BeNull();
            authContextResponse.AuthUrl.Should().NotBeNull();
            Guid authContextId = authContextResponse.Id;
            string authUrl = authContextResponse.AuthUrl!;

            // GET auth context
            DomesticPaymentConsentAuthContextReadResponse authContextResponse2 =
                await requestBuilder.PaymentInitiation.DomesticPaymentConsents
                    .AuthContexts
                    .ReadLocalAsync(authContextId);
            // Checks
            authContextResponse2.Should().NotBeNull();
            authContextResponse2.Warnings.Should().BeNull();

            // Consent authorisation
            if (consentAuth is not null)
            {
                // Authorise consent in UI via Playwright
                await consentAuth.AuthoriseAsync(
                    authUrl,
                    bankProfile,
                    ConsentVariety.DomesticPaymentConsent,
                    bankUser);

                // Refresh scope to ensure user token acquired following consent is available
                using IRequestBuilderContainer scopedRequestBuilderNew = requestBuilderGenerator();
                IRequestBuilder requestBuilderNew = scopedRequestBuilderNew.RequestBuilder;

                if (paymentInitiationApiSettings.UseDomesticPaymentConsentGetFundsConfirmationEndpoint)
                {
                    // GET consent funds confirmation
                    DomesticPaymentConsentReadFundsConfirmationResponse domesticPaymentConsentResp4 =
                        await requestBuilderNew.PaymentInitiation.DomesticPaymentConsents
                            .ReadFundsConfirmationAsync(domesticPaymentConsentId);

                    // Checks
                    domesticPaymentConsentResp4.Should().NotBeNull();
                    domesticPaymentConsentResp4.Warnings.Should().BeNull();
                    domesticPaymentConsentResp4.ExternalApiResponse.Should().NotBeNull();
                }

                // POST domestic payment
                await pispFluentRequestLogging
                    .AppendToPath("domesticPayment")
                    .AppendToPath("postRequest")
                    .WriteFile(domesticPaymentRequest);
                domesticPaymentRequest.ExternalApiRequest.Data.ConsentId = "";
                domesticPaymentRequest.ExternalApiRequest.Data.Initiation.InstructionIdentification =
                    domesticPaymentConsentRequest.ExternalApiRequest.Data.Initiation.InstructionIdentification;
                domesticPaymentRequest.ExternalApiRequest.Data.Initiation.EndToEndIdentification =
                    domesticPaymentConsentRequest.ExternalApiRequest.Data.Initiation.EndToEndIdentification;
                domesticPaymentRequest.Reference = testNameUnique;
                DomesticPaymentResponse domesticPaymentResp =
                    await requestBuilderNew.PaymentInitiation.DomesticPayments
                        .CreateAsync(domesticPaymentRequest, domesticPaymentConsentId);

                // Checks
                domesticPaymentResp.Should().NotBeNull();
                domesticPaymentResp.Warnings.Should().BeNull();
                domesticPaymentResp.ExternalApiResponse.Should().NotBeNull();
                string domesticPaymentExternalId = domesticPaymentResp.ExternalApiResponse.Data.DomesticPaymentId;

                // GET domestic payment
                DomesticPaymentResponse domesticPaymentResp2 =
                    await requestBuilderNew.PaymentInitiation.DomesticPayments
                        .ReadAsync(domesticPaymentExternalId, domesticPaymentConsentId);

                // Checks
                domesticPaymentResp2.Should().NotBeNull();
                domesticPaymentResp2.Warnings.Should().BeNull();
                domesticPaymentResp2.ExternalApiResponse.Should().NotBeNull();
            }

            // DELETE domestic payment consent
            ObjectDeleteResponse domesticPaymentConsentResp3 = await requestBuilder
                .PaymentInitiation
                .DomesticPaymentConsents
                .DeleteLocalAsync(domesticPaymentConsentId, modifiedBy);

            // Checks
            domesticPaymentConsentResp3.Should().NotBeNull();
            domesticPaymentConsentResp3.Warnings.Should().BeNull();

            // DELETE API object
            ObjectDeleteResponse apiResponse = await requestBuilder
                .BankConfiguration
                .PaymentInitiationApis
                .DeleteLocalAsync(paymentInitiationApiId, modifiedBy);

            // Checks
            apiResponse.Should().NotBeNull();
            apiResponse.Warnings.Should().BeNull();
        }
    }
}
