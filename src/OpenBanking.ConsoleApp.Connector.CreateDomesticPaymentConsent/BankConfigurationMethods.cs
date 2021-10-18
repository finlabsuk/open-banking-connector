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
    ///     Bank configuration methods
    /// </summary>
    public static class BankConfigurationMethods
    {
        /// <summary>
        ///     Create bank configuration including bank, bank API information and
        ///     bank registration objects
        /// </summary>
        public static async Task<(Guid bankId, Guid bankRegistrationId, Guid bankApiSetId)> Create(
            string softwareStatementProfileId,
            RegistrationScope registrationScope,
            IRequestBuilder requestBuilder,
            BankProfile bankProfile,
            string testNameUnique)
        {
            // Create bank
            Bank bankRequest = bankProfile.BankRequest(testNameUnique);
            IFluentResponse<BankResponse> bankResp = await requestBuilder
                .BankConfiguration
                .Banks
                .PostLocalAsync(bankRequest);
            Guid bankId = bankResp.Data!.Id;

            // Create bank registration
            BankRegistration registrationRequest = bankProfile.BankRegistrationRequest(
                testNameUnique,
                bankId,
                softwareStatementProfileId,
                registrationScope,
                null);
            IFluentResponse<BankRegistrationResponse> registrationResp = await requestBuilder
                .BankConfiguration
                .BankRegistrations
                .PostAsync(registrationRequest);
            Guid bankRegistrationId = registrationResp.Data!.Id;

            // Create bank API information
            BankApiSet apiSetRequest = bankProfile.BankApiSetRequest(
                testNameUnique,
                bankId);
            IFluentResponse<BankApiSetResponse> apiInformationResponse = await requestBuilder
                .BankConfiguration
                .BankApiSets
                .PostLocalAsync(apiSetRequest);
            Guid bankApiSetId = apiInformationResponse.Data!.Id;

            // Return IDs of created objects
            return (bankId, bankRegistrationId, bankApiSetId);
        }
    }
}
