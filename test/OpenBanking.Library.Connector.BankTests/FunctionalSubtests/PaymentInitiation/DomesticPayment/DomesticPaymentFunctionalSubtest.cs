// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FluentAssertions;
using Jering.Javascript.NodeJS;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.PaymentInitiation.DomesticPayment
{
    public delegate DomesticPaymentConsent DomesticPaymentConsentDelegate(BankProfile bankProfile, Guid bankId);

    public partial class DomesticPaymentFunctionalSubtest
    {
        public DomesticPaymentFunctionalSubtest(
            DomesticPaymentFunctionalSubtestEnum domesticPaymentFunctionalSubtestEnum)
        {
            DomesticPaymentFunctionalSubtestEnum = domesticPaymentFunctionalSubtestEnum;
        }

        public DomesticPaymentFunctionalSubtestEnum DomesticPaymentFunctionalSubtestEnum { get; }

        public static ISet<DomesticPaymentFunctionalSubtestEnum> DomesticPaymentFunctionalSubtestsSupported(
            BankProfile bankProfile) => DomesticPaymentFunctionalSubtestHelper.AllDomesticPaymentFunctionalTests;

        public static async Task RunTest(
            DomesticPaymentFunctionalSubtestEnum subtestEnum,
            BankProfile bankProfile,
            Guid bankRegistrationId,
            Guid bankApiInformationId,
            IRequestBuilder requestBuilder,
            TestDataProcessor testDataProcessor,
            bool includeConsentAuth,
            INodeJSService? nodeJsService,
            PuppeteerLaunchOptions? puppeteerLaunchOptions)
        {
            DomesticPaymentFunctionalSubtest subtest = DomesticPaymentFunctionalSubtestHelper.Test(subtestEnum);

            // POST domestic payment consent
            DomesticPaymentConsent domesticConsentPaymentRequest = new DomesticPaymentConsent
            {
                WriteDomesticConsent = DomesticPaymentConsent(
                    subtest.DomesticPaymentFunctionalSubtestEnum,
                    "placeholder: OBC consent ID",
                    "placeholder: random GUID"),
                BankApiInformationId = Guid.Empty,
                BankRegistrationId = Guid.Empty
            };
            domesticConsentPaymentRequest =
                bankProfile.DomesticPaymentConsentAdjustments(domesticConsentPaymentRequest);
            await testDataProcessor.ProcessData(domesticConsentPaymentRequest);
            domesticConsentPaymentRequest.BankRegistrationId = bankRegistrationId;
            domesticConsentPaymentRequest.BankApiInformationId = bankApiInformationId;
            domesticConsentPaymentRequest.WriteDomesticConsent.Data.Initiation.InstructionIdentification =
                Guid.NewGuid().ToString("N");
            domesticConsentPaymentRequest.WriteDomesticConsent.Data.Initiation.EndToEndIdentification =
                Guid.NewGuid().ToString("N");

            IFluentResponse<DomesticPaymentConsentPostResponse> postConsentResp =
                await requestBuilder.PaymentInitiation.DomesticPaymentConsents
                    .PostAsync(domesticConsentPaymentRequest);

            postConsentResp.Should().NotBeNull();
            postConsentResp.Messages.Should().BeEmpty();
            postConsentResp.Data.Should().NotBeNull();
            postConsentResp.Data!.AuthUrl.Should().NotBeNull();

            // GET domestic payment consent
            IFluentResponse<DomesticPaymentConsentGetResponse> getConsentResponse =
                await requestBuilder.PaymentInitiation.DomesticPaymentConsents
                    .GetAsync(postConsentResp.Data!.Id);
            getConsentResponse.Should().NotBeNull();
            getConsentResponse.Messages.Should().BeEmpty();
            getConsentResponse.Data.Should().NotBeNull();

            if (includeConsentAuth)
            {
                if (puppeteerLaunchOptions is null ||
                    nodeJsService is null)
                {
                    throw new ArgumentNullException($"{nameof(puppeteerLaunchOptions)} or {nameof(nodeJsService)}");
                }

                // Automated consent authorisation
                Guid consentId = postConsentResp.Data!.Id;
                object[] args =
                {
                    postConsentResp.Data!.AuthUrl!,
                    bankProfile.BankProfileEnum.ToString(),
                    "DomesticPaymentConsent",
                    bankProfile.BankUser,
                    puppeteerLaunchOptions
                };
                await nodeJsService.InvokeFromFileAsync(
                    "authoriseConsent.js",
                    "authoriseConsent",
                    args);

                // POST domestic payment
                Models.Public.PaymentInitiation.Request.DomesticPayment domesticPaymentRequest =
                    new Models.Public.PaymentInitiation.Request.DomesticPayment
                        { DomesticPaymentConsentId = default };
                await testDataProcessor.ProcessData(domesticPaymentRequest);
                domesticPaymentRequest.DomesticPaymentConsentId = consentId;
                IFluentResponse<DomesticPaymentPostResponse> paymentResp =
                    await requestBuilder.PaymentInitiation.DomesticPayments
                        .PostAsync(domesticPaymentRequest);

                // Checks
                paymentResp.Should().NotBeNull();
                paymentResp.Messages.Should().BeEmpty();
                paymentResp.Data.Should().NotBeNull();
            }
        }
    }
}
