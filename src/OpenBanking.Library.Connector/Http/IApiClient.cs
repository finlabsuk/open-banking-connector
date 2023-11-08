// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

public interface IApiClient
{
    Task<T> SendExpectingJsonResponseAsync<T>(
        HttpRequestMessage request,
        JsonSerializerSettings? jsonSerializerSettings)
        where T : class;

    Task SendExpectingNoResponseAsync(HttpRequestMessage request);
    Task<string> SendExpectingStringResponseAsync(HttpRequestMessage request);
}
