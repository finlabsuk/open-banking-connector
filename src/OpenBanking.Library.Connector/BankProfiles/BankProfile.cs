// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;

public delegate bool RegistrationScopeIsValid(RegistrationScopeEnum registrationScope);

public delegate AccountAndTransactionModelsPublic.OBReadConsent1 AccountAccessConsentExternalApiRequestAdjustments(
    AccountAndTransactionModelsPublic.OBReadConsent1 externalApiRequest);

public delegate PaymentInitiationModelsPublic.OBWriteDomesticConsent4
    DomesticPaymentConsentExternalApiRequestAdjustments(
        PaymentInitiationModelsPublic.OBWriteDomesticConsent4 externalApiRequest);

public delegate DomesticVrpConsentRequest DomesticVrpConsentAdjustments(DomesticVrpConsentRequest domesticVrpConsent);

public class BankConfigurationApiSettings
{
    /// <summary>
    ///     Describes whether a registration scope should be used when testing with this bank.
    ///     By default, any registration scope can be used.
    /// </summary>
    public RegistrationScopeIsValid RegistrationScopeIsValid { get; set; } = registrationScope => true;

    /// <summary>
    ///     Describes whether registration endpoints used when testing with this bank.
    /// </summary>
    public bool UseRegistrationEndpoints { get; set; } = true;

    /// <summary>
    ///     Describes whether DELETE /register/{ClientId} is used when testing with this bank.
    /// </summary>
    public bool UseRegistrationDeleteEndpoint { get; set; } = false;

    public bool ProcessedUseRegistrationDeleteEndpoint =>
        UseRegistrationEndpoints && UseRegistrationDeleteEndpoint;

    /// <summary>
    ///     Describes whether GET /register/{ClientId} is used when testing with this bank.
    /// </summary>
    public bool UseRegistrationGetEndpoint { get; set; } = false;

    public bool ProcessedUseRegistrationGetEndpoint =>
        UseRegistrationEndpoints && UseRegistrationGetEndpoint;

    public bool UseRegistrationAccessToken { get; set; } = false;

    public TokenEndpointAuthMethod TokenEndpointAuthMethod { get; set; } = TokenEndpointAuthMethod.TlsClientAuth;

    /// <summary>
    ///     ID token "sub" claim type.
    /// </summary>
    public IdTokenSubClaimType IdTokenSubClaimType { get; set; } = IdTokenSubClaimType.ConsentId;

    /// <summary>
    ///     Bank registration group.
    /// </summary>
    public BankRegistrationGroup? BankRegistrationGroup = null;
}

public class AccountAndTransactionApiSettings
{
    /// <summary>
    ///     Describes whether GET /accounts/{AccountId}/party is used when testing with this bank
    /// </summary>
    public bool UseGetPartyEndpoint { get; set; } = true;

    public AccountAccessConsentExternalApiRequestAdjustments
        AccountAccessConsentExternalApiRequestAdjustments { get; set; } = x => x;
}

public class PaymentInitiationApiSettings
{
    public bool UseDomesticPaymentConsentGetFundsConfirmationEndpoint { get; set; } = true;

    public DomesticPaymentConsentExternalApiRequestAdjustments
        DomesticPaymentConsentExternalApiRequestAdjustments { get; set; } = x => x;
}

public class VariableRecurringPaymentsApiSettings
{
    public bool UseConsentGetFundsConfirmationEndpoint { get; set; } = true;

    public DomesticVrpConsentAdjustments
        DomesticVrpConsentAdjustments { get; set; } = x => x;
}

/// <summary>
///     A Bank Profile describes configuration and settings used when testing with a particular bank.
///     It also provides convenient functions for generating request objects to use with Open Banking Connector.
/// </summary>
public partial class BankProfile
{
    public BankProfile(
        BankProfileEnum bankProfileEnum,
        string issuerUrl,
        string financialId,
        AccountAndTransactionApi? accountAndTransactionApi,
        PaymentInitiationApi? paymentInitiationApi,
        VariableRecurringPaymentsApi? variableRecurringPaymentsApi,
        bool supportsSca)
    {
        BankProfileEnum = bankProfileEnum;
        IssuerUrl = issuerUrl;
        FinancialId = financialId ?? throw new ArgumentNullException(nameof(financialId));
        AccountAndTransactionApi = accountAndTransactionApi;
        PaymentInitiationApi = paymentInitiationApi;
        VariableRecurringPaymentsApi = variableRecurringPaymentsApi;
        SupportsSca = supportsSca;
    }

    /// <summary>
    ///     Identifier enum used for this bank
    /// </summary>
    public BankProfileEnum BankProfileEnum { get; }

    /// <summary>
    ///     Bank issuer URL which points to well-known endpoint
    /// </summary>
    public string IssuerUrl { get; }

    /// <summary>
    ///     Bank financial ID used in UK Open Banking
    /// </summary>
    public string FinancialId { get; }

    /// <summary>
    ///     Client Registration (DCR) API version.
    /// </summary>
    public DynamicClientRegistrationApiVersion DynamicClientRegistrationApiVersion { get; set; } =
        DynamicClientRegistrationApiVersion.Version3p2;

    /// <summary>
    ///     Account and Transaction (AISP) API version. May be null where API not supported or used/tested.
    /// </summary>
    public AccountAndTransactionApi? AccountAndTransactionApi { get; }

    /// <summary>
    ///     Payment Initiation (PISP) API version. May be null where API not supported or used/tested.
    /// </summary>
    public PaymentInitiationApi? PaymentInitiationApi { get; }

    /// <summary>
    ///     Variable Recurring Payments (VRP) API version. May be null where API not supported or used/tested.
    /// </summary>
    public VariableRecurringPaymentsApi? VariableRecurringPaymentsApi { get; }

    public OAuth2ResponseMode DefaultResponseMode { get; set; } = OAuth2ResponseMode.Fragment;

    public bool SupportsSca { get; }

    public CustomBehaviourClass? CustomBehaviour { get; set; }

    /// <summary>
    ///     Settings used when testing Client Registration API.
    /// </summary>
    public BankConfigurationApiSettings BankConfigurationApiSettings { get; set; } =
        new();

    /// <summary>
    ///     Settings used when testing Account and Transaction API.
    /// </summary>
    public AccountAndTransactionApiSettings AccountAndTransactionApiSettings { get; set; } =
        new();

    /// <summary>
    ///     Settings used when testing Payment Initiation API.
    /// </summary>
    public PaymentInitiationApiSettings PaymentInitiationApiSettings { get; set; } =
        new();

    /// <summary>
    ///     Settings used when testing Variable Recurring Payments API.
    /// </summary>
    public VariableRecurringPaymentsApiSettings VariableRecurringPaymentsApiSettings { get; set; } =
        new();
}
