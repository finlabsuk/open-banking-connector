// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.AccountTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Public
{
    [PersistenceEquivalent(typeof(Persistent.BankClientProfile))]
    public class BankClientProfile
    {
        [JsonProperty("bankClient")]
        public BankClient BankClient { get; set; }

        [JsonProperty("accountTransactionApiVersion")]
        public AccountApiVersion AccountTransactionApiVersion { get; set; }

        [JsonProperty("accountTransactionApiBaseUrl")]
        public string AccountTransactionApiBaseUrl { get; set; }

        [JsonProperty("paymentInitiationApiVersion")]
        public ApiVersion PaymentInitiationApiVersion { get; set; }

        [JsonProperty("paymentInitiationApiBaseUrl")]
        public string PaymentInitiationApiBaseUrl { get; set; }

        [JsonProperty("accountTransactionApiSettingsOverrides")]
        public ApiSettingsOverrides AccountTransactionApiSettingsOverrides { get; set; }
    }
}
