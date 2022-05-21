// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests
{
    public class TestConfiguration
    {
        [JsonProperty("mockHttp")]
        public string MockHttp { get; set; } = null!;

        [JsonProperty("softwareStatement")]
        public string SoftwareStatement { get; set; } = null!;

        [JsonProperty("signingKeyId")]
        public string SigningKeyId { get; set; } = null!;

        [JsonProperty("signingCertificateKey")]
        public string SigningCertificateKey { get; set; } = null!;

        [JsonProperty("signingCertificate")]
        public string SigningCertificate { get; set; } = null!;

        [JsonProperty("transportCertificateKey")]
        public string TransportCertificateKey { get; set; } = null!;

        [JsonProperty("transportCertificate")]
        public string TransportCertificate { get; set; } = null!;

        [JsonProperty("defaultfragmentredirecturl")]
        public string DefaultFragmentRedirectUrl { get; set; } = null!;

        [JsonProperty("xFapiFinancialId")]
        public string XFapiFinancialId { get; set; } = null!;

        [JsonProperty("clientProfileIssuerUrl")]
        public string ClientProfileIssuerURl { get; set; } = null!;

        [JsonProperty("accountApiVersion")]
        public string AccountApiVersion { get; set; } = null!;

        [JsonProperty("accountApiUrl")]
        public string AccountApiUrl { get; set; } = null!;

        [JsonProperty("paymentApiVersion")]
        public string PaymentApiVersion { get; set; } = null!;

        [JsonProperty("paymentApiUrl")]
        public string PaymentApiUrl { get; set; } = null!;

        [JsonProperty("openBankingClientRegistrationClaimsOverrides")]
        public BankRegistrationPostCustomBehaviour OpenBankingClientRegistrationClaimsOverrides { get; set; } = null!;

        [JsonProperty("openBankingOpenIdConfiguration")]
        public OpenIdConfiguration OpenIdConfiguration { get; set; } = null!;
    }
}
