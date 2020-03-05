﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Entities
{
    public class PostOpenBankingClientProfileResponse
    {
        public PostOpenBankingClientProfileResponse(BankClientProfile data, MessagesResponse messages)
        {
            Data = data;
            Messages = messages;
        }

        [JsonProperty("data")]
        public BankClientProfile Data { get; set; }

        [JsonProperty("messages")]
        public MessagesResponse Messages { get; set; }
    }
}
