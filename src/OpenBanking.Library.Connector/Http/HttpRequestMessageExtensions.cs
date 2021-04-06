// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http
{
    internal static class HttpRequestMessageExtensions
    {
        public static Task<T> RequestJsonAsync<T>(
            this HttpRequestMessage request,
            IApiClient client,
            bool requestContentIsJson,
            JsonSerializerSettings? jsonSerializerSettings = null,
            bool typeTIsNullable = false)
            where T : class?
        {
            request.ArgNotNull(nameof(request));
            client.ArgNotNull(nameof(client));

            return client.RequestJsonAsync<T>(
                request,
                requestContentIsJson,
                jsonSerializerSettings,
                typeTIsNullable);
        }
    }
}
