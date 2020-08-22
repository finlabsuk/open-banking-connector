// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests
{
    public class TestConfiguration
    {
        [JsonProperty("mockHttp")]
        public string MockHttp { get; set; }

        [JsonProperty("softwareStatement")]
        public string SoftwareStatement { get; set; }

        [JsonProperty("signingKeyId")]
        public string SigningKeyId { get; set; }

        [JsonProperty("signingCertificateKey")]
        public string SigningCertificateKey { get; set; }

        [JsonProperty("signingCertificate")]
        public string SigningCertificate { get; set; }

        [JsonProperty("transportCertificateKey")]
        public string TransportCertificateKey { get; set; }

        [JsonProperty("transportCertificate")]
        public string TransportCertificate { get; set; }

        [JsonProperty("defaultfragmentredirecturl")]
        public string DefaultFragmentRedirectUrl { get; set; }

        [JsonProperty("xFapiFinancialId")]
        public string XFapiFinancialId { get; set; }

        [JsonProperty("clientProfileIssuerUrl")]
        public string ClientProfileIssuerURl { get; set; }

        [JsonProperty("accountApiVersion")]
        public string AccountApiVersion { get; set; }

        [JsonProperty("accountApiUrl")]
        public string AccountApiUrl { get; set; }

        [JsonProperty("paymentApiVersion")]
        public string PaymentApiVersion { get; set; }

        [JsonProperty("paymentApiUrl")]
        public string PaymentApiUrl { get; set; }

        [JsonProperty("openBankingClientRegistrationClaimsOverrides")]
        public BankClientRegistrationClaimsOverrides OpenBankingClientRegistrationClaimsOverrides { get; set; }

        [JsonProperty("openBankingOpenIdConfiguration")]
        public OpenIdConfiguration OpenIdConfiguration { get; set; }
    }
}
