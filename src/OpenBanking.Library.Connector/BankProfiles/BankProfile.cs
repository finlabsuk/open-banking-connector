// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.ClientRegistration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles
{
    public delegate BankRegistration BankRegistrationAdjustments(
        BankRegistration bankRegistration,
        RegistrationScope registrationScope);

    public delegate bool UseRegistrationScope(RegistrationScope registrationScope);

    public delegate DomesticPaymentConsent DomesticPaymentConsentAdjustments(
        DomesticPaymentConsent domesticPaymentConsent);

    public delegate DomesticVrpConsent DomesticVrpConsentAdjustments(DomesticVrpConsent domesticVrpConsent);

    public class ClientRegistrationApiSettings
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
            (x, _) => x;

        /// <summary>
        ///     Describes whether DELETE /register/{ClientId} is used when testing with this bank.
        /// </summary>
        public bool UseDeleteEndpoint { get; set; } = false;

        public bool UseRegistrationAccessToken { get; set; } = false;
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
            ClientRegistrationApiVersion clientRegistrationApiVersion,
            PaymentInitiationApi? paymentInitiationApi)
        {
            BankProfileEnum = bankProfileEnum;
            IssuerUrl = issuerUrl ?? throw new ArgumentNullException(nameof(issuerUrl));
            FinancialId = financialId ?? throw new ArgumentNullException(nameof(financialId));
            PaymentInitiationApi = paymentInitiationApi;
            ClientRegistrationApiVersion = clientRegistrationApiVersion;
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
        public ClientRegistrationApiVersion ClientRegistrationApiVersion { get; }

        /// <summary>
        ///     Payment Initiation (PISP) API version. May be null because this API may not be used in testing.
        /// </summary>
        public PaymentInitiationApi? PaymentInitiationApi { get; }

        /// <summary>
        ///     Settings used when testing Client Registration API.
        /// </summary>
        public ClientRegistrationApiSettings ClientRegistrationApiSettings { get; set; } =
            new ClientRegistrationApiSettings();

        /// <summary>
        ///     Settings used when testing Payment Initiation API.
        /// </summary>
        public PaymentInitiationApiSettings PaymentInitiationApiSettings { get; set; } =
            new PaymentInitiationApiSettings();

        /// <summary>
        ///     Settings used when testing Variable Recurring Payments API.
        /// </summary>
        public VariableRecurringPaymentsApiSettings VariableRecurringPaymentsApiSettings { get; set; } =
            new VariableRecurringPaymentsApiSettings();
    }
}
