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
        public OpenIdConfigurationGetCustomBehaviour? OpenIdConfigurationGet { get; set; }
        public BankRegistrationPostCustomBehaviour? BankRegistrationPost { get; set; }
        public ConsentAuthGetCustomBehaviour? AccountAccessConsentAuthGet { get; set; }
        public ConsentAuthGetCustomBehaviour? DomesticPaymentConsentAuthGet { get; set; }
        public ConsentAuthGetCustomBehaviour? DomesticVrpConsentAuthGet { get; set; }
    }
}
