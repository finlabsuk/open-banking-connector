// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration
{
    /// <summary>
    ///     Custom behaviour, usually bank-specific, to handle quirks, formatting issues, etc.
    ///     For a well-behaved bank, this object should be full of nulls (empty).
    /// </summary>
    public class CustomBehaviour
    {
        /// <summary>
        ///     JSON string with Open ID configuration (well-known endpoint) response to be used
        ///     instead of calling bank API. When null, bank API endpoint provides response.
        /// </summary>
        public string? OpenIdConfigurationReplacement { get; set; } = null;

        public OpenIdConfigurationOverrides? OpenIdConfigurationOverrides { get; set; }

        public BankRegistrationClaimsOverrides? BankRegistrationClaimsOverrides { get; set; }

        public BankRegistrationClaimsJsonOptions? BankRegistrationClaimsJsonOptions { get; set; }

        public BankRegistrationResponseOverrides? BankRegistrationResponseOverrides { get; set; }


        public OAuth2RequestObjectClaimsOverrides? OAuth2RequestObjectClaimsOverrides { get; set; }
    }
}
