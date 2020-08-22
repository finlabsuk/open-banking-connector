// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http
{
    public interface IApiClient
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

        Task<T> RequestJsonAsync<T>(
            HttpRequestMessage request,
            bool requestContentIsJson,
            JsonSerializerSettings? jsonSerializerSettings)
            where T : class;
    }
}
