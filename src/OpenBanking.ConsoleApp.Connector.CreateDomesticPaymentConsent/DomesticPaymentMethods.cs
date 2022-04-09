// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        public static async Task<string> Create(
            DomesticPaymentConsent domesticPaymentConsentRequest,
            Guid domesticPaymentConsentId,
            IRequestBuilder requestBuilder,
            string testNameUnique)
        {
            // Create domestic payment request
            requestBuilder.Utility.Map(
                domesticPaymentConsentRequest.ExternalApiRequest,
                out PaymentInitiationModelsPublic.OBWriteDomestic2
                    obWriteDomestic); // maps Open Banking request objects
            var domesticPaymentRequest =
                new DomesticPayment
                {
                    ExternalApiRequest = obWriteDomestic,
                    Name = testNameUnique
                };

            // POST domestic payment
            IFluentResponse<DomesticPaymentResponse> domesticPaymentResponse =
                await requestBuilder
                    .PaymentInitiation
                    .DomesticPayments
                    .CreateAsync(domesticPaymentRequest, domesticPaymentConsentId);
            string domesticPaymentId = domesticPaymentResponse.Data!.ExternalApiResponse.Data.DomesticPaymentId;

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
            IFluentResponse<DomesticPaymentConsentReadResponse> domesticPaymentResponse =
                await requestBuilder
                    .PaymentInitiation
                    .DomesticPaymentConsents
                    .ReadAsync(domesticPaymentId);
        }
    }
}
