// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles
{
    /// <summary>
    ///     Describes <see cref="PaymentInitiationApi" /> hidden properties object in bankProfileHiddenProperties.json
    ///     Properties are nullable versions (since optional) of those in <see cref="PaymentInitiationApi" />.
    /// </summary>
    public class PaymentInitiationApiHiddenProperties
    {
        public PaymentInitiationApiVersion? PaymentInitiationApiVersion { get; set; }

        public string? BaseUrl { get; set; }
    }

    /// <summary>
    ///     Describes <see cref="VariableRecurringPaymentsApi" /> hidden properties object in bankProfileHiddenProperties.json
    ///     Properties are nullable versions (since optional) of those in <see cref="VariableRecurringPaymentsApi" />.
    /// </summary>
    public class VariableRecurringPaymentsApiHiddenProperties
    {
        public VariableRecurringPaymentsApiVersion? VariableRecurringPaymentsApiVersion { get; set; }

        public string? BaseUrl { get; set; }
    }

    /// <summary>
    ///     Describes <see cref="BankProfile" /> hidden properties object in bankProfileHiddenProperties.json.
    ///     Properties are nullable versions (since optional) of those in <see cref="BankProfile" />.
    /// </summary>
    public class BankProfileHiddenProperties
    {
        public string? IssuerUrl { get; set; }

        public string? FinancialId { get; set; }

        public DynamicClientRegistrationApiVersion? DefaultClientRegistrationApiVersion { get; set; }

        public PaymentInitiationApiHiddenProperties? DefaultPaymentInitiationApi { get; set; }

        public VariableRecurringPaymentsApiHiddenProperties? DefaultVariableRecurringPaymentsApi { get; set; }

        public string? AdditionalProperty1 { get; set; }

        public string? AdditionalProperty2 { get; set; }

        public string GetRequiredIssuerUrl() =>
            IssuerUrl ?? throw new Exception("No issuer URL");

        public string GetRequiredFinancialId() =>
            FinancialId ?? throw new Exception("No financial ID");

        public DynamicClientRegistrationApiVersion GetRequiredClientRegistrationApiVersion() =>
            DefaultClientRegistrationApiVersion ?? throw new Exception("No ClientRegistrationApiVersion");

        public PaymentInitiationApiVersion GetRequiredPaymentInitiationApiVersion() =>
            DefaultPaymentInitiationApi?.PaymentInitiationApiVersion ??
            throw new Exception("No PISP API version");

        public string GetRequiredPaymentInitiationApiBaseUrl() =>
            DefaultPaymentInitiationApi?.BaseUrl ?? throw new Exception("No PISP base URL");

        public VariableRecurringPaymentsApiVersion GetRequiredVariableRecurringPaymentsApiVersion() =>
            DefaultVariableRecurringPaymentsApi?.VariableRecurringPaymentsApiVersion ??
            throw new Exception("No VRP API version");

        public string GetRequiredVariableRecurringPaymentsApiBaseUrl() =>
            DefaultVariableRecurringPaymentsApi?.BaseUrl ?? throw new Exception("No VRP base URL");

        public string GetAdditionalProperty1() =>
            AdditionalProperty1 ?? throw new Exception($"Hidden property not found: {AdditionalProperty1}");

        public string GetAdditionalProperty2() =>
            AdditionalProperty2 ?? throw new Exception($"Hidden property not found: {AdditionalProperty2}");
    }
}
