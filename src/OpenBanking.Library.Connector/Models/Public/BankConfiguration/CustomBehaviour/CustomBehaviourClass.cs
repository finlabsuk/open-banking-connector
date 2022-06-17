// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour
{
    /// <summary>
    ///     Custom behaviour, usually bank-specific, to handle quirks, formatting issues, etc.
    ///     For a well-behaved bank, this object should be full of nulls (empty).
    /// </summary>
    public class CustomBehaviourClass
    {
        // Non-endpoint
        //public HttpMtlsCustomBehaviour? HttpMtls { get; set; }

        // Bank config endpoints
        public OpenIdConfigurationGetCustomBehaviour? OpenIdConfigurationGet { get; set; }
        public BankRegistrationPostCustomBehaviour? BankRegistrationPost { get; set; }

        // Client auth 
        public GrantPostCustomBehaviour? ClientCredentialsGrantPost { get; set; }

        // Consent auth endpoints
        public JwksGetCustomBehaviour? JwksGet { get; set; }
        public ConsentAuthGetCustomBehaviour? AccountAccessConsentAuthGet { get; set; }
        public ConsentAuthGetCustomBehaviour? DomesticPaymentConsentAuthGet { get; set; }
        public ConsentAuthGetCustomBehaviour? DomesticVrpConsentAuthGet { get; set; }
        public GrantPostCustomBehaviour? AuthCodeGrantPost { get; set; }
        public GrantPostCustomBehaviour? RefreshTokenGrantPost { get; set; }
    }
}
