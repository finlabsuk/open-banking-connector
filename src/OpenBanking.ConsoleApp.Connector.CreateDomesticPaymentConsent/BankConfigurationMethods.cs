// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.RequestObjects.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;

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
        public static async Task<(Guid bankId, Guid bankRegistrationId )> Create(
            string softwareStatementProfileId,
            string? softwareStatementAndCertificateProfileOverrideCase,
            RegistrationScopeEnum registrationScope,
            IRequestBuilder requestBuilder,
            BankProfile bankProfile,
            string testNameUnique)
        {
            // Create bank
            Bank bankRequest = bankProfile.GetBankRequest();
            BankResponse bankResp = await requestBuilder
                .BankConfiguration
                .Banks
                .CreateLocalAsync(bankRequest);
            Guid bankId = bankResp.Id;

            // Create bank registration
            BankRegistration registrationRequest = bankProfile.GetBankRegistrationRequest(
                bankId,
                softwareStatementProfileId,
                softwareStatementAndCertificateProfileOverrideCase,
                registrationScope);
            BankRegistrationResponse registrationResp = await requestBuilder
                .BankConfiguration
                .BankRegistrations
                .CreateAsync(registrationRequest);
            Guid bankRegistrationId = registrationResp.Id;

            // Return IDs of created objects
            return (bankId, bankRegistrationId);
        }
    }
}
