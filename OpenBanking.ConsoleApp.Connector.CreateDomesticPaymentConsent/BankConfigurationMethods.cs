// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.ConsoleApp.Connector.CreateDomesticPaymentConsent
{
    /// <summary>
    ///     Class for creating bank registration
    /// </summary>
    public static class BankConfigurationMethods
    {
        /// <summary>
        ///     Create bank registration including bank, bank API information and
        ///     bank registration objects
        /// </summary>
        public static async Task<(Guid bankId, Guid bankRegistrationId, Guid bankApiInformationId)> Create(
            string softwareStatementProfileId,
            RegistrationScope registrationScope,
            IRequestBuilder requestBuilder,
            BankProfile bankProfile,
            string testNameUnique)
        {
            // Create bank
            Bank bankRequest = bankProfile.BankRequest(testNameUnique);
            IFluentResponse<BankResponse> bankResp = await requestBuilder
                .ClientRegistration
                .Banks
                .PostLocalAsync(bankRequest);
            Guid bankId = bankResp.Data!.Id;

            // Create bank registration
            BankRegistration registrationRequest = bankProfile.BankRegistrationRequest(
                testNameUnique,
                bankId,
                softwareStatementProfileId,
                registrationScope);
            IFluentResponse<BankRegistrationResponse> registrationResp = await requestBuilder.ClientRegistration
                .BankRegistrations
                .PostAsync(registrationRequest);
            Guid bankRegistrationId = registrationResp.Data!.Id;

            // Create bank API information
            BankApiInformation apiInformationRequest = bankProfile.BankApiInformationRequest(
                testNameUnique,
                bankId);
            IFluentResponse<BankApiInformationResponse> apiInformationResponse = await requestBuilder
                .ClientRegistration
                .BankApiInformationObjects
                .PostLocalAsync(apiInformationRequest);
            Guid bankApiInformationId = apiInformationResponse.Data!.Id;

            return (bankId, bankRegistrationId, bankApiInformationId);
        }
    }
}
