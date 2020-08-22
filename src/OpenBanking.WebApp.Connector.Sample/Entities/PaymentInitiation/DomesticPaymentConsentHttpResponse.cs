﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost.Entities;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Entities.PaymentInitiation
{
    public class DomesticPaymentConsentHttpResponse
    {
        public DomesticPaymentConsentHttpResponse(MessagesResponse messages, DomesticPaymentConsentResponse data)
        {
            Messages = messages;
            Data = data;
        }

        [JsonProperty("messages")]
        public MessagesResponse Messages { get; set; }

        [JsonProperty("data")]
        public DomesticPaymentConsentResponse Data { get; set; }
    }
}
