// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;

public delegate bool RegistrationScopeIsValid(RegistrationScopeEnum registrationScope);

public delegate AccountAndTransactionModelsPublic.OBReadConsent1 AccountAccessConsentExternalApiRequestAdjustments(
    AccountAndTransactionModelsPublic.OBReadConsent1 externalApiRequest);

public delegate PaymentInitiationModelsPublic.OBWriteDomesticConsent4
    DomesticPaymentConsentExternalApiRequestAdjustments(
        PaymentInitiationModelsPublic.OBWriteDomesticConsent4 externalApiRequest);

public delegate PaymentInitiationModelsPublic.OBWriteDomestic2
    DomesticPaymentExternalApiRequestAdjustments(PaymentInitiationModelsPublic.OBWriteDomestic2 externalApiRequest);

public delegate VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest
    DomesticVrpConsentExternalApiRequestAdjustments(
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest externalApiRequest);

public delegate VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest
    DomesticVrpExternalApiRequestAdjustments(
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest externalApiRequest);

public class BankConfigurationApiSettings
{
    private readonly bool _useRegistrationDeleteEndpoint;
    private readonly bool _useRegistrationEndpoint = true;
    private readonly bool _useRegistrationGetEndpoint;

    /// <summary>
    ///     Describes whether a registration scope should be used when testing with this bank.
    ///     By default, any registration scope can be used.
    /// </summary>
    public RegistrationScopeIsValid RegistrationScopeIsValid { get; set; } = registrationScope => true;

    /// <summary>
    ///     Describes whether registration endpoints used when testing with this bank.
    /// </summary>
    public bool UseRegistrationEndpoint
    {
        get => _useRegistrationEndpoint;
        init
        {
            if (!value &&
                (UseRegistrationDeleteEndpoint || UseRegistrationGetEndpoint))
            {
                throw new InvalidOperationException(
                    $"Can't set {nameof(UseRegistrationEndpoint)} " +
                    $"to false when either {nameof(UseRegistrationDeleteEndpoint)} " +
                    $"or {nameof(UseRegistrationGetEndpoint)} is true.");
            }

            _useRegistrationEndpoint = value;
        }
    }

    /// <summary>
    ///     Describes whether DELETE /register/{ClientId} is used when testing with this bank.
    /// </summary>
    public bool UseRegistrationDeleteEndpoint
    {
        get => _useRegistrationDeleteEndpoint;
        init
        {
            if (value && !UseRegistrationEndpoint)
            {
                throw new InvalidOperationException(
                    $"Can't set {nameof(UseRegistrationDeleteEndpoint)} " +
                    $"to true when {nameof(UseRegistrationEndpoint)} is false.");
            }

            _useRegistrationDeleteEndpoint = value;
        }
    }

    /// <summary>
    ///     Describes whether GET /register/{ClientId} is used when testing with this bank.
    /// </summary>
    public bool UseRegistrationGetEndpoint
    {
        get => _useRegistrationGetEndpoint;
        init
        {
            if (value && !UseRegistrationEndpoint)
            {
                throw new InvalidOperationException(
                    $"Can't set {nameof(UseRegistrationGetEndpoint)} " +
                    $"to true when {nameof(UseRegistrationEndpoint)} is false.");
            }

            _useRegistrationGetEndpoint = value;
        }
    }

    public bool TestTemporaryBankRegistration { get; set; }

    public bool UseRegistrationAccessToken { get; set; }

    public TokenEndpointAuthMethodSupportedValues TokenEndpointAuthMethod { get; set; } =
        TokenEndpointAuthMethodSupportedValues.TlsClientAuth;

    /// <summary>
    ///     ID token "sub" claim type.
    /// </summary>
    public IdTokenSubClaimType IdTokenSubClaimType { get; set; } = IdTokenSubClaimType.ConsentId;
}

public class AccountAndTransactionApiSettings
{
    /// <summary>
    ///     Describes whether GET /accounts/{AccountId}/party is used when testing with this bank
    /// </summary>
    public bool UseGetPartyEndpoint { get; set; } = true;

    public bool UseReauth { get; set; } = true;

    public AccountAccessConsentExternalApiRequestAdjustments
        AccountAccessConsentExternalApiRequestAdjustments { get; set; } = x => x;
}

public class PaymentInitiationApiSettings
{
    public bool UseDomesticPaymentConsentGetFundsConfirmationEndpoint { get; set; } = true;

    public DomesticPaymentConsentExternalApiRequestAdjustments
        DomesticPaymentConsentExternalApiRequestAdjustments { get; set; } = x => x;

    public DomesticPaymentExternalApiRequestAdjustments
        DomesticPaymentExternalApiRequestAdjustments { get; set; } = x => x;
}

public class VariableRecurringPaymentsApiSettings
{
    public bool UseConsentGetFundsConfirmationEndpoint { get; set; } = true;

    public DomesticVrpConsentExternalApiRequestAdjustments
        DomesticVrpConsentExternalApiRequestAdjustments { get; set; } = x => x;

    public DomesticVrpExternalApiRequestAdjustments
        DomesticVrpExternalApiRequestAdjustments { get; set; } = x => x;
}

/// <summary>
///     A Bank Profile describes configuration and settings used when testing with a particular bank.
///     It also provides convenient functions for generating request objects to use with Open Banking Connector.
/// </summary>
public class BankProfile
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

    public AccountAndTransactionApi GetRequiredAccountAndTransactionApi() =>
        AccountAndTransactionApi ??
        throw new InvalidOperationException(
            $"No Open Banking Account and Transaction (AISP) API associated with BankProfile ${BankProfileEnum}.");

    public PaymentInitiationApi GetRequiredPaymentInitiationApi() =>
        PaymentInitiationApi ??
        throw new InvalidOperationException(
            $"No Open Banking Payment Initiation (PISP) API associated with BankProfile ${BankProfileEnum}.");

    public VariableRecurringPaymentsApi GetRequiredVariableRecurringPaymentsApi() =>
        VariableRecurringPaymentsApi ??
        throw new InvalidOperationException(
            $"No Open Banking Variable Recurring Payments (VRP) API associated with BankProfile ${BankProfileEnum}.");
}
