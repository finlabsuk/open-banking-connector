// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.RequestObjects.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;

namespace FinnovationLabs.OpenBanking.ConsoleApp.Connector.CreateDomesticPaymentConsent
{
    /// <summary>
    ///     Domestic VRP consent methods
    /// </summary>
    public static class DomesticVrpConsentMethods
    {
        /// <summary>
        ///     Create VRP consent
        /// </summary>
        /// <param name="bankProfile"></param>
        /// <param name="bankRegistrationId"></param>
        /// <param name="variableRecurringPaymentsApi"></param>
        /// <param name="domesticVrpTypeEnum"></param>
        /// <param name="requestBuilder"></param>
        /// <param name="testNameUnique"></param>
        /// <returns></returns>
        public static async Task<Guid> Create(
            BankProfile bankProfile,
            Guid bankRegistrationId,
            Guid variableRecurringPaymentsApi,
            DomesticVrpTypeEnum domesticVrpTypeEnum,
            IRequestBuilder requestBuilder,
            string testNameUnique)
        {
            // Create domestic VRP consent request
            DomesticVrpConsent domesticVrpConsentRequest =
                bankProfile.DomesticVrpConsentRequest(
                    bankRegistrationId,
                    variableRecurringPaymentsApi,
                    domesticVrpTypeEnum);

            // POST domestic payment consent
            DomesticVrpConsentResponse domesticVrpConsentResponse =
                await requestBuilder
                    .VariableRecurringPayments
                    .DomesticVrpConsents
                    .CreateAsync(domesticVrpConsentRequest);

            Guid domesticVrpConsentId = domesticVrpConsentResponse.Id;

            // Return ID of created object
            return domesticVrpConsentId;
        }

        /// <summary>
        ///     Read domestic VRP consent
        /// </summary>
        /// <param name="requestBuilder"></param>
        /// <param name="domesticVrpConsentId"></param>
        public static async Task Read(
            IRequestBuilder requestBuilder,
            Guid domesticVrpConsentId)
        {
            // GET domestic VRP consent
            DomesticVrpConsentResponse domesticVrpConsentResponse =
                await requestBuilder
                    .VariableRecurringPayments
                    .DomesticVrpConsents
                    .ReadAsync(domesticVrpConsentId);
        }

        /// <summary>
        ///     Read domestic VRP consent funds confirmation
        /// </summary>
        /// <param name="requestBuilder"></param>
        /// <param name="domesticVrpConsentId"></param>
        public static async Task ReadFundsConfirmation(
            IRequestBuilder requestBuilder,
            Guid domesticVrpConsentId)
        {
            // GET domestic payment consent
            DomesticVrpConsentReadFundsConfirmationResponse domesticVrpConsentResponse =
                await requestBuilder
                    .VariableRecurringPayments
                    .DomesticVrpConsents
                    .ReadFundsConfirmationAsync(domesticVrpConsentId);
        }
    }
}
