// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request
{
    public class PaymentInitiationApiProfile
    {
        public PaymentInitiationApiProfile(string id, string bankClientProfileId, ApiVersion apiVersion, string baseUrl)
        {
            Id = id;
            BankClientProfileId = bankClientProfileId;
            ApiVersion = apiVersion;
            BaseUrl = baseUrl;
        }

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("bankClientProfileId")]
        public string BankClientProfileId { get; }

        [JsonProperty("apiVersion")]
        public ApiVersion ApiVersion { get; }

        [JsonProperty("baseUrl")]
        public string BaseUrl { get; }
    }
}
