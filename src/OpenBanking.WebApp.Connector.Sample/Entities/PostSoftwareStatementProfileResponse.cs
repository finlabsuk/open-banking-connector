﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Entities
{
    public class PostSoftwareStatementProfileResponse
    {
        public PostSoftwareStatementProfileResponse(MessagesResponse messages)
        {
            Messages = messages;
        }

        [JsonProperty("messages")]
        public MessagesResponse Messages { get; set; }
    }
}
