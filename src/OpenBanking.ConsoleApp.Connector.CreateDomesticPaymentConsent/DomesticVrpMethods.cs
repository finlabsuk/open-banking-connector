// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.ConsoleApp.Connector.CreateDomesticPaymentConsent
{
    /// <summary>
    ///     Domestic VRP methods
    /// </summary>
    public static class DomesticVrpMethods
    {
        /// <summary>
        ///     Create domestic VRP
        /// </summary>
        /// <param name="domesticVrpConsentRequest"></param>
        /// <param name="domesticVrpConsentId"></param>
        /// <param name="requestBuilder"></param>
        /// <param name="testNameUnique"></param>
        /// <returns></returns>
        public static async Task<string> Create(
            DomesticVrpConsent domesticVrpConsentRequest,
            Guid domesticVrpConsentId,
            IRequestBuilder requestBuilder,
            string testNameUnique)
        {
            // Create domestic VRP request
            requestBuilder.Utility.Map(
                domesticVrpConsentRequest.ExternalApiRequest,
                out VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest
                    obDomesticVrpRequest); // maps Open Banking request objects
            var domesticVrpRequest =
                new DomesticVrp
                {
                    ExternalApiRequest = obDomesticVrpRequest,
                    Name = testNameUnique,
                };

            // POST domestic VRP
            IFluentResponse<DomesticVrpResponse> domesticVrpResponse =
                await requestBuilder
                    .VariableRecurringPayments
                    .DomesticVrps
                    .CreateAsync(domesticVrpRequest, domesticVrpConsentId);
            string domesticVrpId = domesticVrpResponse.Data!.ExternalApiResponse.Data.DomesticVRPId;

            // Return ID of created object
            return domesticVrpId;
        }

        /// <summary>
        ///     Read domestic VRP
        /// </summary>
        /// <param name="requestBuilder"></param>
        /// <param name="domesticVrpId"></param>
        public static async Task Read(
            IRequestBuilder requestBuilder,
            Guid domesticVrpId)
        {
            // GET domestic payment consent
            IFluentResponse<DomesticVrpConsentReadResponse> domesticVrpResponse =
                await requestBuilder
                    .VariableRecurringPayments
                    .DomesticVrpConsents
                    .ReadAsync(domesticVrpId);
        }
    }
}
