// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    [PersistenceEquivalent(typeof(Persistent.BankClientProfile))]
    public class BankClientProfile
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("softwareStatementProfileId")]
        public string SoftwareStatementProfileId { get; set; }

        [JsonProperty("issuerUrl")]
        public string IssuerUrl { get; set; }

        [JsonProperty("xFapiFinancialId")]
        public string XFapiFinancialId { get; set; }

        [JsonProperty("openIdOverrides")]
        public OpenIdConfigurationOverrides OpenIdConfigurationOverrides { get; set; }

        [JsonProperty("httpMtlsOverrides")]
        public HttpMtlsConfigurationOverrides HttpMtlsConfigurationOverrides { get; set; }

        [JsonProperty("bankClientRegistrationClaimsOverrides")]
        public BankClientRegistrationClaimsOverrides BankClientRegistrationClaimsOverrides { get; set; }

        [JsonProperty("registrationResponseOverrides")]
        public BankClientRegistrationDataOverrides BankClientRegistrationDataOverrides { get; set; }
    }
}
