// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;

namespace FinnovationLabs.OpenBanking.ConsoleApp.Connector.CreateDomesticPaymentConsent
{
    /// <summary>
    ///     Domestic payment methods
    /// </summary>
    public static class DomesticPaymentMethods
    {
        /// <summary>
        ///     Create domestic payment
        /// </summary>
        /// <param name="domesticPaymentConsentRequest"></param>
        /// <param name="domesticPaymentConsentId"></param>
        /// <param name="requestBuilder"></param>
        /// <param name="testNameUnique"></param>
        /// <returns></returns>
        public static async Task<Guid> Create(
            DomesticPaymentConsent domesticPaymentConsentRequest,
            Guid domesticPaymentConsentId,
            IRequestBuilder requestBuilder,
            string testNameUnique)
        {
            // Create domestic payment request
            requestBuilder.Utility.Map(
                domesticPaymentConsentRequest.OBWriteDomesticConsent,
                out PaymentInitiationModelsPublic.OBWriteDomestic2
                    obWriteDomestic); // maps Open Banking request objects
            var domesticPaymentRequest =
                new DomesticPayment
                {
                    OBWriteDomestic = obWriteDomestic,
                    DomesticPaymentConsentId = domesticPaymentConsentId,
                    Name = testNameUnique
                };

            // POST domestic payment
            IFluentResponse<DomesticPaymentResponse> domesticPaymentResponse =
                await requestBuilder
                    .PaymentInitiation
                    .DomesticPayments
                    .PostAsync(domesticPaymentRequest);
            Guid domesticPaymentId = domesticPaymentResponse.Data!.Id;

            // Return ID of created object
            return domesticPaymentId;
        }

        /// <summary>
        ///     Read domestic payment
        /// </summary>
        /// <param name="requestBuilder"></param>
        /// <param name="domesticPaymentId"></param>
        public static async Task Read(
            IRequestBuilder requestBuilder,
            Guid domesticPaymentId)
        {
            // GET domestic payment consent
            IFluentResponse<DomesticPaymentConsentResponse> domesticPaymentResponse =
                await requestBuilder
                    .PaymentInitiation
                    .DomesticPaymentConsents
                    .GetAsync(domesticPaymentId);
        }
    }
}
