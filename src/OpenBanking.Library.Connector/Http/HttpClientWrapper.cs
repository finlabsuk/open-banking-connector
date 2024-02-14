// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

public class HttpClientWrapper(HttpClient httpClient) : IHttpClient
{
    public Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken) => httpClient.SendAsync(request, completionOption, cancellationToken);

    public void Dispose()
    {
        httpClient.Dispose();
    }
}
