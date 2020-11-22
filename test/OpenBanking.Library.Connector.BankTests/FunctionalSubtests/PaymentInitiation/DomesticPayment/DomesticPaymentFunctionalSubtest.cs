// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
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
            Guid bankId,
            IRequestBuilder requestBuilder,
            TestDataProcessor testDataProcessor,
            INodeJSService nodeJsService)
        {
            DomesticPaymentFunctionalSubtest subtest = DomesticPaymentFunctionalSubtestHelper.Test(subtestEnum);

            // POST domestic payment consent
            DomesticPaymentConsent domesticConsentPaymentRequest =
                subtest.DomesticPaymentConsent(bankProfile: bankProfile, bankId: default);
            testDataProcessor.ProcessData(domesticConsentPaymentRequest);
            domesticConsentPaymentRequest.BankId = bankId;
            FluentResponse<DomesticPaymentConsentResponse> consentResp =
                await requestBuilder.PaymentInitiation.DomesticPaymentConsents
                    .PostAsync(domesticConsentPaymentRequest);

            // Checks
            consentResp.Should().NotBeNull();
            consentResp.Messages.Should().BeEmpty();
            consentResp.Data.Should().NotBeNull();
            Guid consentId = consentResp.Data!.Id;

            // Automated consent authorisation
            await nodeJsService.InvokeFromFileAsync(
                modulePath: "authoriseConsent.js",
                exportName: "authoriseConsent",
                args: new[]
                {
                    consentResp.Data!.AuthUrl, bankProfile.BankProfileEnum.ToString(), "DomesticPaymentConsent"
                });

            // POST domestic payment
            Models.Public.PaymentInitiation.Request.DomesticPayment domesticPaymentRequest =
                new Models.Public.PaymentInitiation.Request.DomesticPayment
                    { ConsentId = default };
            testDataProcessor.ProcessData(domesticPaymentRequest);
            domesticPaymentRequest.ConsentId = consentId;
            FluentResponse<DomesticPaymentResponse> paymentResp =
                await requestBuilder.PaymentInitiation.DomesticPayments
                    .PostAsync(domesticPaymentRequest);

            // Checks
            paymentResp.Should().NotBeNull();
            paymentResp.Messages.Should().BeEmpty();
            paymentResp.Data.Should().NotBeNull();
        }
    }
}
