﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.WebHost.Models.Public.Response
{
    /// <summary>
    ///     HTTP response for request that does not return data
    /// </summary>
    public class HttpResponse
    {
        public HttpResponse(HttpResponseMessages? messages)
        {
            Messages = messages;
        }

        [JsonProperty("messages")]
        public HttpResponseMessages? Messages { get; }
    }

    /// <summary>
    ///     HTTP response for request that returns data
    /// </summary>
    public class HttpResponse<TData>
        where TData : class
    {
        public HttpResponse(HttpResponseMessages? messages, TData? data)
        {
            Messages = messages;
            Data = data;
        }

        [JsonProperty("messages")]
        public HttpResponseMessages? Messages { get; }

        [JsonProperty("data")]
        public TData? Data { get; }
    }
}
