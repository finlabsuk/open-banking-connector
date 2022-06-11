// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

public interface IOpenIdConfigurationRead
{
    Task<OpenIdConfiguration> GetOpenIdConfigurationAsync(string issuerUrl);
}

public class OpenIdConfigurationRead : IOpenIdConfigurationRead
{
    private readonly IApiClient _apiClient;

    public OpenIdConfigurationRead(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<OpenIdConfiguration> GetOpenIdConfigurationAsync(string issuerUrl)
    {
        var uri = new Uri(string.Join("/", issuerUrl.TrimEnd('/'), ".well-known/openid-configuration"));

        return await new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(uri)
            .Create()
            .RequestJsonAsync<OpenIdConfiguration>(_apiClient);
    }
}
