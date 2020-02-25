// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.AccountTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent
{
    public class OpenBankingClientProfile
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("openBankingClientId")]
        public string OpenBankingClientId { get; set; }

        [JsonProperty("redirectUrl")]
        public string RedirectUrl { get; set; }

        [JsonProperty("account_transaction_api_version")]
        public AccountApiVersion AccountTransactionApiVersion { get; set; }

        [JsonProperty("account_transaction_api_base_url")]
        public string AccountTransactionApiBaseUrl { get; set; }

        [JsonProperty("payment_initiation_api_version")]
        public ApiVersion PaymentInitiationApiVersion { get; set; }

        [JsonProperty("payment_initiation_api_base_url")]
        public string PaymentInitiationApiBaseUrl { get; set; }
    }
}
