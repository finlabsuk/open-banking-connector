// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FluentAssertions;
using Jering.Javascript.NodeJS;
using DomesticPaymentConsentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPaymentConsent;
using DomesticPaymentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPayment;
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
                : DomesticPaymentFunctionalSubtestHelper.AllDomesticPaymentSubtests;

        public static async Task RunTest(
            DomesticPaymentSubtestEnum subtestEnum,
            BankProfile bankProfile,
            Guid bankRegistrationId,
            Guid bankApiSetId,
            PaymentInitiationApiSettings paymentInitiationApiSettings,
            IRequestBuilder requestBuilderIn,
            Func<IRequestBuilderContainer> requestBuilderGenerator,
            string testNameUnique,
            FilePathBuilder testDataProcessorFluentRequestLogging,
            bool includeConsentAuth,
            INodeJSService? nodeJsService,
            PuppeteerLaunchOptionsJavaScript? puppeteerLaunchOptions,
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

            // Basic request object for domestic payment consent
            DomesticPaymentConsentRequest domesticPaymentConsentRequest =
                bankProfile.DomesticPaymentConsentRequest(
                    Guid.Empty,
                    Guid.Empty,
                    DomesticPaymentFunctionalSubtestHelper.DomesticPaymentType(subtestEnum),
                    "placeholder: random GUID",
                    "placeholder: random GUID",
                    null);
            await testDataProcessorFluentRequestLogging
                .AppendToPath("domesticPaymentConsent")
                .AppendToPath("postRequest")
                .WriteFile(domesticPaymentConsentRequest);

            // Basic request object for domestic payment
            requestBuilder.Utility.Map(
                domesticPaymentConsentRequest.OBWriteDomesticConsent,
                out PaymentInitiationModelsPublic.OBWriteDomestic2 obWriteDomestic);
            var domesticPaymentRequest =
                new DomesticPaymentRequest
                {
                    OBWriteDomestic = obWriteDomestic,
                    DomesticPaymentConsentId = default,
                    Name = null
                };

            // POST domestic payment consent
            domesticPaymentConsentRequest.BankRegistrationId = bankRegistrationId;
            domesticPaymentConsentRequest.BankApiSetId = bankApiSetId;
            domesticPaymentConsentRequest.OBWriteDomesticConsent.Data.Initiation.InstructionIdentification =
                Guid.NewGuid().ToString("N");
            domesticPaymentConsentRequest.OBWriteDomesticConsent.Data.Initiation.EndToEndIdentification =
                Guid.NewGuid().ToString("N");
            domesticPaymentConsentRequest.Name = testNameUnique;
            IFluentResponse<DomesticPaymentConsentResponse> domesticPaymentConsentResp =
                await requestBuilder.PaymentInitiation.DomesticPaymentConsents
                    .PostAsync(domesticPaymentConsentRequest);

            // Checks
            domesticPaymentConsentResp.Should().NotBeNull();
            domesticPaymentConsentResp.Messages.Should().BeEmpty();
            domesticPaymentConsentResp.Data.Should().NotBeNull();
            Guid domesticPaymentConsentId = domesticPaymentConsentResp.Data!.Id;

            // GET domestic payment consent
            IFluentResponse<DomesticPaymentConsentResponse> domesticPaymentConsentResp2 =
                await requestBuilder.PaymentInitiation.DomesticPaymentConsents
                    .GetAsync(domesticPaymentConsentId);

            // Checks
            domesticPaymentConsentResp2.Should().NotBeNull();
            domesticPaymentConsentResp2.Messages.Should().BeEmpty();
            domesticPaymentConsentResp2.Data.Should().NotBeNull();

            // POST auth context
            var authContextRequest = new DomesticPaymentConsentAuthContext
            {
                DomesticPaymentConsentId = domesticPaymentConsentId,
                Name = testNameUnique
            };
            IFluentResponse<DomesticPaymentConsentAuthContextPostResponse> authContextResponse =
                await requestBuilder.PaymentInitiation
                    .DomesticPaymentConsents
                    .AuthContexts
                    .PostLocalAsync(authContextRequest);

            // Checks
            authContextResponse.Should().NotBeNull();
            authContextResponse.Messages.Should().BeEmpty();
            authContextResponse.Data.Should().NotBeNull();
            authContextResponse.Data!.AuthUrl.Should().NotBeNull();
            Guid authContextId = authContextResponse.Data!.Id;
            string authUrl = authContextResponse.Data!.AuthUrl!;

            // GET auth context
            IFluentResponse<DomesticPaymentConsentAuthContextResponse> authContextResponse2 =
                await requestBuilder.PaymentInitiation.DomesticPaymentConsents
                    .AuthContexts
                    .GetLocalAsync(authContextId);
            // Checks
            authContextResponse2.Should().NotBeNull();
            authContextResponse2.Messages.Should().BeEmpty();
            authContextResponse2.Data.Should().NotBeNull();

            // Consent authorisation
            if (includeConsentAuth)
            {
                if (puppeteerLaunchOptions is null ||
                    nodeJsService is null)
                {
                    throw new ArgumentNullException($"{nameof(puppeteerLaunchOptions)} or {nameof(nodeJsService)}");
                }

                // Call Node JS to authorise consent in UI via Puppeteer
                object[] args =
                {
                    authUrl,
                    bankProfile.BankProfileEnum.ToString(),
                    "DomesticPaymentConsent",
                    bankUser,
                    puppeteerLaunchOptions
                };
                await nodeJsService.InvokeFromFileAsync(
                    "authoriseConsent.js",
                    "authoriseConsent",
                    args);

                // Refresh scope to ensure user token acquired following consent is available
                using IRequestBuilderContainer scopedRequestBuilderNew = requestBuilderGenerator();
                IRequestBuilder requestBuilderNew = scopedRequestBuilderNew.RequestBuilder;

                if (paymentInitiationApiSettings.UseConsentGetFundsConfirmationEndpoint)
                {
                    // GET consent funds confirmation
                    IFluentResponse<DomesticPaymentConsentResponse> domesticPaymentConsentResp4 =
                        await requestBuilderNew.PaymentInitiation.DomesticPaymentConsents
                            .GetFundsConfirmationAsync(domesticPaymentConsentId);

                    // Checks
                    domesticPaymentConsentResp4.Should().NotBeNull();
                    domesticPaymentConsentResp4.Messages.Should().BeEmpty();
                    domesticPaymentConsentResp4.Data.Should().NotBeNull();
                }

                // POST domestic payment
                await testDataProcessorFluentRequestLogging
                    .AppendToPath("domesticPayment")
                    .AppendToPath("postRequest")
                    .WriteFile(domesticPaymentRequest);
                domesticPaymentRequest.DomesticPaymentConsentId = domesticPaymentConsentId;
                domesticPaymentRequest.OBWriteDomestic.Data.Initiation.InstructionIdentification =
                    domesticPaymentConsentRequest.OBWriteDomesticConsent.Data.Initiation.InstructionIdentification;
                domesticPaymentRequest.OBWriteDomestic.Data.Initiation.EndToEndIdentification =
                    domesticPaymentConsentRequest.OBWriteDomesticConsent.Data.Initiation.EndToEndIdentification;
                domesticPaymentRequest.Name = testNameUnique;
                IFluentResponse<DomesticPaymentResponse> domesticPaymentResp =
                    await requestBuilderNew.PaymentInitiation.DomesticPayments
                        .PostAsync(domesticPaymentRequest);

                // Checks
                domesticPaymentResp.Should().NotBeNull();
                domesticPaymentResp.Messages.Should().BeEmpty();
                domesticPaymentResp.Data.Should().NotBeNull();
                Guid domesticPaymentId = domesticPaymentResp.Data!.Id;

                // GET domestic payment
                IFluentResponse<DomesticPaymentResponse> domesticPaymentResp2 =
                    await requestBuilderNew.PaymentInitiation.DomesticPayments
                        .GetAsync(domesticPaymentId);

                // Checks
                domesticPaymentResp2.Should().NotBeNull();
                domesticPaymentResp2.Messages.Should().BeEmpty();
                domesticPaymentResp2.Data.Should().NotBeNull();

                // DELETE domestic payment
                IFluentResponse domesticPaymentResp3 = await requestBuilderNew.PaymentInitiation
                    .DomesticPayments
                    .DeleteLocalAsync(domesticPaymentId);

                // Checks
                domesticPaymentResp3.Should().NotBeNull();
                domesticPaymentResp3.Messages.Should().BeEmpty();
            }

            // DELETE auth context


            // DELETE domestic payment consent
            IFluentResponse domesticPaymentConsentResp3 = await requestBuilder.PaymentInitiation
                .DomesticPaymentConsents
                .DeleteLocalAsync(domesticPaymentConsentId);

            // Checks
            domesticPaymentConsentResp3.Should().NotBeNull();
            domesticPaymentConsentResp3.Messages.Should().BeEmpty();
        }
    }
}
