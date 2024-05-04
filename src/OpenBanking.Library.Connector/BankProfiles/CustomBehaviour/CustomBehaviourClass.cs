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

    public AuthCodeGrantPostCustomBehaviour? AccountAccessConsentAuthCodeGrantPost { get; set; }

    public AuthCodeGrantPostCustomBehaviour? DomesticPaymentConsentAuthCodeGrantPost { get; set; }

    public AuthCodeGrantPostCustomBehaviour? DomesticVrpConsentAuthCodeGrantPost { get; set; }

    public RefreshTokenGrantPostCustomBehaviour? AccountAccessConsentRefreshTokenGrantPost { get; set; }

    public RefreshTokenGrantPostCustomBehaviour? DomesticPaymentConsentRefreshTokenGrantPost { get; set; }

    public RefreshTokenGrantPostCustomBehaviour? DomesticVrpConsentRefreshTokenGrantPost { get; set; }

    // Account and Transaction API endpoints
    public ReadWritePostCustomBehaviour? AccountAccessConsentPost { get; set; }

    public ReadWriteGetCustomBehaviour? AccountAccessConsentGet { get; set; }

    public ReadWriteGetCustomBehaviour? AccountGet { get; set; }

    public ReadWriteGetCustomBehaviour? BalanceGet { get; set; }

    public DirectDebitGetCustomBehaviour? DirectDebitGet { get; set; }

    public ReadWriteGetCustomBehaviour? MonzoPotGet { get; set; }

    public ReadWriteGetCustomBehaviour? Party2Get { get; set; }

    public ReadWriteGetCustomBehaviour? PartyGet { get; set; }

    public ReadWriteGetCustomBehaviour? StandingOrderGet { get; set; }

    public ReadWriteGetCustomBehaviour? TransactionGet { get; set; }

    // Payment Initiation API endpoints
    public ReadWritePostCustomBehaviour? DomesticPaymentConsentPost { get; set; }

    public ReadWriteGetCustomBehaviour? DomesticPaymentConsentGet { get; set; }

    public ReadWritePostCustomBehaviour? DomesticPaymentPost { get; set; }

    public ReadWriteGetCustomBehaviour? DomesticPaymentGet { get; set; }

    // Variable Recurring Payments API endpoints
    public ReadWritePostCustomBehaviour? DomesticVrpConsentPost { get; set; }

    public ReadWriteGetCustomBehaviour? DomesticVrpConsentGet { get; set; }

    public ReadWritePostCustomBehaviour? DomesticVrpPost { get; set; }

    public ReadWriteGetCustomBehaviour? DomesticVrpGet { get; set; }
}
