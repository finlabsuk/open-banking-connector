// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;

namespace FinnovationLabs.OpenBanking.ConsoleApp.Connector.CreateDomesticPaymentConsent
{
    public static class DomesticPaymentConsentMethods
    {
        public static async Task<Guid> Create(
            BankProfile bankProfile,
            Guid bankRegistrationId,
            Guid bankApiInformationId,
            IRequestBuilder requestBuilder,
            string testNameUnique)
        {
            // POST domestic payment consent
            var domesticConsentPaymentRequest = new DomesticPaymentConsent
            {
                WriteDomesticConsent = bankProfile.DomesticPaymentConsent(
                    DomesticPaymentTypeEnum.PersonToMerchant,
                    Guid.NewGuid().ToString("N"),
                    Guid.NewGuid().ToString("N")),
                BankApiInformationId = bankApiInformationId,
                BankRegistrationId = bankRegistrationId,
                Name = testNameUnique
            };
            domesticConsentPaymentRequest =
                bankProfile.PaymentInitiationApiSettings.DomesticPaymentConsentAdjustments(
                    domesticConsentPaymentRequest);
            IFluentResponse<DomesticPaymentConsentResponse> domesticPaymentConsentResp =
                await requestBuilder.PaymentInitiation.DomesticPaymentConsents
                    .PostAsync(domesticConsentPaymentRequest);
            Guid domesticPaymentConsentId = domesticPaymentConsentResp.Data!.Id;

            return domesticPaymentConsentId;
        }

        public static async Task Get(
            IRequestBuilder requestBuilder,
            Guid domesticPaymentConsentId)
        {
            // GET domestic payment consent
            IFluentResponse<DomesticPaymentConsentResponse> domesticPaymentConsentResp2 =
                await requestBuilder.PaymentInitiation.DomesticPaymentConsents
                    .GetAsync(domesticPaymentConsentId);
        }
    }
}
