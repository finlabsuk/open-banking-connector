// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.ClientRegistration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles
{
    /// <summary>
    ///     Describes <see cref="PaymentInitiationApiVersion" /> hidden properties object in bankProfileHiddenProperties.json
    ///     Properties are nullable versions (since optional) of those in <see cref="PaymentInitiationApiVersion" />.
    /// </summary>
    public class PaymentInitiationApiHiddenProperties
    {
        public PaymentInitiationApiVersion? PaymentInitiationApiVersion { get; set; }

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

        public ClientRegistrationApiVersion? DefaultClientRegistrationApiVersion { get; set; }

        public PaymentInitiationApiHiddenProperties? DefaultPaymentInitiationApi { get; set; }

        public string GetRequiredPaymentInitiationApiBaseUrl() =>
            DefaultPaymentInitiationApi?.BaseUrl ?? throw new Exception("No base URL");

        public string GetRequiredIssuerUrl() =>
            IssuerUrl ?? throw new Exception("No issuer URL");

        public string GetRequiredFinancialId() =>
            FinancialId ?? throw new Exception("No financial ID");

        public ClientRegistrationApiVersion GetRequiredClientRegistrationApiVersion() =>
            DefaultClientRegistrationApiVersion ?? throw new Exception("No ClientRegistrationApiVersion");

        public PaymentInitiationApiVersion GetRequiredPaymentInitiationApiVersion() =>
            DefaultPaymentInitiationApi?.PaymentInitiationApiVersion ??
            throw new Exception("No PaymentInitiationApiVersion");
    }
}
