// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;

/// <summary>
///     Custom behaviour, usually bank-specific, to handle quirks, formatting issues, etc.
///     For a well-behaved bank, this object should be full of nulls (empty).
/// </summary>
public class CustomBehaviourClass
{
    // Bank config endpoints
    public OpenIdConfigurationGetCustomBehaviour? OpenIdConfigurationGet { get; set; }
    public BankRegistrationPostCustomBehaviour? BankRegistrationPost { get; set; }

    public BankRegistrationPutCustomBehaviour? BankRegistrationPut { get; set; }

    // Client auth 
    public ClientCredentialsGrantPostCustomBehaviour? ClientCredentialsGrantPost { get; set; }

    // Consent auth endpoints
    public JwksGetCustomBehaviour? JwksGet { get; set; }
    public ConsentAuthGetCustomBehaviour? AccountAccessConsentAuthGet { get; set; }
    public ConsentAuthGetCustomBehaviour? DomesticPaymentConsentAuthGet { get; set; }
    public ConsentAuthGetCustomBehaviour? DomesticVrpConsentAuthGet { get; set; }
    public AuthCodeAndRefreshTokenGrantPostCustomBehaviour? AuthCodeGrantPost { get; set; }
    public AuthCodeAndRefreshTokenGrantPostCustomBehaviour? RefreshTokenGrantPost { get; set; }

    // Account and Transaction API endpoints
    public AccountAccessConsentPostCustomBehaviour? AccountAccessConsentPost { get; set; }

    public AccountAccessConsentGetCustomBehaviour? AccountAccessConsentGet { get; set; }

    public DirectDebitGetCustomBehaviour? DirectDebitGet { get; set; }
}
