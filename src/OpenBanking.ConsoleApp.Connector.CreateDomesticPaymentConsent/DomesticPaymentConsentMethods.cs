// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;

namespace FinnovationLabs.OpenBanking.ConsoleApp.Connector.CreateDomesticPaymentConsent
{
    /// <summary>
    ///     Domestic payment consent methods
    /// </summary>
    public static class DomesticPaymentConsentMethods
    {
        /// <summary>
        ///     Create domestic payment consent
        /// </summary>
        /// <param name="bankProfile"></param>
        /// <param name="bankRegistrationId"></param>
        /// <param name="paymentInitiationApiId"></param>
        /// <param name="endToEndIdentification"></param>
        /// <param name="requestBuilder"></param>
        /// <param name="testNameUnique"></param>
        /// <param name="domesticPaymentTemplateType"></param>
        /// <param name="instructionIdentification"></param>
        /// <returns></returns>
        public static async Task<Guid> Create(
            BankProfile bankProfile,
            Guid bankRegistrationId,
            Guid paymentInitiationApiId,
            DomesticPaymentTemplateType domesticPaymentTemplateType,
            string instructionIdentification,
            string endToEndIdentification,
            IRequestBuilder requestBuilder,
            string testNameUnique)
        {
            // Create domestic payment consent request
            var domesticPaymentConsentRequest = new DomesticPaymentConsentRequest
            {
                BankProfile = bankProfile.BankProfileEnum,
                BankRegistrationId = bankRegistrationId, // substitute logging placeholder
                PaymentInitiationApiId = paymentInitiationApiId, // substitute logging placeholder
                TemplateRequest = new DomesticPaymentTemplateRequest
                {
                    Type = domesticPaymentTemplateType,
                    Parameters = new DomesticPaymentTemplateParameters
                    {
                        InstructionIdentification = instructionIdentification,
                        EndToEndIdentification = endToEndIdentification
                    }
                }
            };

            // POST domestic payment consent
            DomesticPaymentConsentCreateResponse domesticPaymentConsentResponse =
                await requestBuilder
                    .PaymentInitiation
                    .DomesticPaymentConsents
                    .CreateAsync(domesticPaymentConsentRequest);
            Guid domesticPaymentConsentId = domesticPaymentConsentResponse.Id;

            // Return ID of created object
            return domesticPaymentConsentId;
        }

        /// <summary>
        ///     Read domestic payment consent
        /// </summary>
        /// <param name="requestBuilder"></param>
        /// <param name="domesticPaymentConsentId"></param>
        public static async Task Read(
            IRequestBuilder requestBuilder,
            Guid domesticPaymentConsentId)
        {
            // GET domestic payment consent
            DomesticPaymentConsentReadResponse domesticPaymentConsentResponse =
                await requestBuilder
                    .PaymentInitiation
                    .DomesticPaymentConsents
                    .ReadAsync(domesticPaymentConsentId);
        }

        /// <summary>
        ///     Read domestic payment consent funds confirmation
        /// </summary>
        /// <param name="requestBuilder"></param>
        /// <param name="domesticPaymentConsentId"></param>
        public static async Task ReadFundsConfirmation(
            IRequestBuilder requestBuilder,
            Guid domesticPaymentConsentId)
        {
            // GET domestic payment consent
            DomesticPaymentConsentReadFundsConfirmationResponse domesticPaymentConsentResponse =
                await requestBuilder
                    .PaymentInitiation
                    .DomesticPaymentConsents
                    .ReadFundsConfirmationAsync(domesticPaymentConsentId);
        }
    }
}
