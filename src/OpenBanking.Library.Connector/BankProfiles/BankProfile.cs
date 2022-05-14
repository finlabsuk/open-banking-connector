// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles
{
    public delegate BankRegistration BankRegistrationAdjustments(BankRegistration bankRegistration);

    public delegate bool UseRegistrationScope(RegistrationScopeEnum registrationScope);

    public delegate AccountAccessConsent AccountAccessConsentAdjustments(AccountAccessConsent domesticPaymentConsent);

    public delegate DomesticPaymentConsent DomesticPaymentConsentAdjustments(
        DomesticPaymentConsent domesticPaymentConsent);

    public delegate DomesticVrpConsent DomesticVrpConsentAdjustments(DomesticVrpConsent domesticVrpConsent);

    public class BankConfigurationApiSettings
    {
        /// <summary>
        ///     Describes whether a registration scope should be used when testing with this bank.
        ///     By default, any registration scope can be used.
        /// </summary>
        public UseRegistrationScope UseRegistrationScope { get; set; } = registrationScope => true;

        /// <summary>
        ///     Adjustments to default BankRegistration request object.
        /// </summary>
        public BankRegistrationAdjustments BankRegistrationAdjustments { get; set; } =
            x => x;

        /// <summary>
        ///     Describes whether DELETE /register/{ClientId} is used when testing with this bank.
        /// </summary>
        public bool UseDeleteEndpoint { get; set; } = false;

        public bool UseRegistrationAccessToken { get; set; } = false;
    }

    public class AccountAndTransactionApiSettings
    {
        public AccountAccessConsentAdjustments
            AccountAccessConsentAdjustments { get; set; } = x => x;
    }

    public class PaymentInitiationApiSettings
    {
        public bool UseConsentGetFundsConfirmationEndpoint { get; set; } = true;

        public DomesticPaymentConsentAdjustments
            DomesticPaymentConsentAdjustments { get; set; } = x => x;
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
            DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion,
            AccountAndTransactionApi? accountAndTransactionApi,
            PaymentInitiationApi? paymentInitiationApi,
            VariableRecurringPaymentsApi? variableRecurringPaymentsApi)
        {
            BankProfileEnum = bankProfileEnum;
            IssuerUrl = issuerUrl ?? throw new ArgumentNullException(nameof(issuerUrl));
            FinancialId = financialId ?? throw new ArgumentNullException(nameof(financialId));
            AccountAndTransactionApi = accountAndTransactionApi;
            PaymentInitiationApi = paymentInitiationApi;
            VariableRecurringPaymentsApi = variableRecurringPaymentsApi;
            DynamicClientRegistrationApiVersion = dynamicClientRegistrationApiVersion;
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
        public DynamicClientRegistrationApiVersion DynamicClientRegistrationApiVersion { get; }

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
}
