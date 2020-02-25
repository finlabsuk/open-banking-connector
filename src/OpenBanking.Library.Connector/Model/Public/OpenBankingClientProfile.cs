// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.AccountTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Public
{
    [PersistenceEquivalent(typeof(Persistent.OpenBankingClientProfile))]
    public class OpenBankingClientProfile
    {
        [JsonProperty("openBankingClient")]
        public OpenBankingClient OpenBankingClient { get; set; }

        [JsonProperty("redirectUrl")]
        public string RedirectUrl { get; set; }

        [JsonProperty("accountApiVersion")]
        public AccountApiVersion AccountTransactionApiVersion { get; set; }

        [JsonProperty("accountApiUrl")]
        public string AccountTransactionApiBaseUrl { get; set; }

        [JsonProperty("paymentApiVersion")]
        public ApiVersion PaymentInitiationApiVersion { get; set; }

        [JsonProperty("paymentApiUrl")]
        public string PaymentInitiationApiBaseUrl { get; set; }

        [JsonProperty("accountApiSettingsOverrides")]
        public ApiSettingsOverrides AccountTransactionApiSettingsOverrides { get; set; }
    }
}
