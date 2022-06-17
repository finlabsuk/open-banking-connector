// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

public interface IOpenIdConfigurationRead
{
    Task<(OpenIdConfiguration openIdConfiguration, IEnumerable<IFluentResponseInfoOrWarningMessage> newNonErrorMessages
            )>
        GetOpenIdConfigurationAsync(
            Uri issuerUrl,
            OpenIdConfigurationGetCustomBehaviour? customBehaviour);
}

public class OpenIdConfigurationRead : IOpenIdConfigurationRead
{
    private readonly IApiClient _apiClient;

    public OpenIdConfigurationRead(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async
        Task<(OpenIdConfiguration openIdConfiguration, IEnumerable<IFluentResponseInfoOrWarningMessage>
            newNonErrorMessages)> GetOpenIdConfigurationAsync(
            Uri openIdConfigurationUrl,
            OpenIdConfigurationGetCustomBehaviour? customBehaviour)
    {
        var openIdConfiguration = await new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(openIdConfigurationUrl)
            .Create()
            .RequestJsonAsync<OpenIdConfiguration>(_apiClient);

        // Update OpenID Provider Configuration based on overrides
        IList<OAuth2ResponseMode>? responseModesSupportedOverride =
            customBehaviour?.ResponseModesSupportedResponse;
        if (!(responseModesSupportedOverride is null))
        {
            openIdConfiguration.ResponseModesSupported = responseModesSupportedOverride;
        }

        IList<OpenIdConfigurationTokenEndpointAuthMethodEnum>? tokenEndpointAuthMethodsSupportedOverride =
            customBehaviour?.TokenEndpointAuthMethodsSupportedResponse;
        if (!(tokenEndpointAuthMethodsSupportedOverride is null))
        {
            openIdConfiguration.TokenEndpointAuthMethodsSupported = tokenEndpointAuthMethodsSupportedOverride;
        }

        // Validate OpenID Connect configuration
        IEnumerable<IFluentResponseInfoOrWarningMessage> newNonErrorMessages =
            new OpenBankingOpenIdConfigurationResponseValidator()
                .Validate(openIdConfiguration)
                .ProcessValidationResultsAndRaiseErrors(messagePrefix: "prefix");

        return (openIdConfiguration, newNonErrorMessages);
    }
}
