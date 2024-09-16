// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi.Validators;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

public interface IOpenIdConfigurationRead
{
    Task<(OpenIdConfiguration? openIdConfiguration, IEnumerable<IFluentResponseInfoOrWarningMessage> newNonErrorMessages
            )>
        GetOpenIdConfigurationAsync(
            string issuerUrl,
            BankProfileEnum bankProfileEnum,
            OpenIdConfigurationGetCustomBehaviour? openIdConfigurationGetCustomBehaviour);
}

public class OpenIdConfigurationRead : IOpenIdConfigurationRead
{
    private readonly IApiClient _apiClient;

    public OpenIdConfigurationRead(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async
        Task<(OpenIdConfiguration? openIdConfiguration, IEnumerable<IFluentResponseInfoOrWarningMessage>
            newNonErrorMessages)> GetOpenIdConfigurationAsync(
            string issuerUrl,
            BankProfileEnum bankProfile,
            OpenIdConfigurationGetCustomBehaviour? openIdConfigurationGetCustomBehaviour)
    {
        if (openIdConfigurationGetCustomBehaviour?.OpenIdConfiguration is not null)
        {
            return (openIdConfigurationGetCustomBehaviour.OpenIdConfiguration,
                new List<IFluentResponseInfoOrWarningMessage>());
        }

        var openIdConfigurationUrl = new Uri(
            openIdConfigurationGetCustomBehaviour?.Url
            ?? string.Join("/", issuerUrl.TrimEnd('/'), ".well-known/openid-configuration"));

        var tppReportingRequestInfo = new TppReportingRequestInfo
        {
            EndpointDescription =
                """
                GET {Issuer}/.well-known/openid-configuration
                """,
            BankProfile = bankProfile
        };
        (OpenIdConfiguration openIdConfiguration, string? xFapiInteractionId) = await new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(openIdConfigurationUrl)
            .SendExpectingJsonResponseAsync<OpenIdConfiguration>(_apiClient, tppReportingRequestInfo);

        // Update OpenID Provider Configuration based on overrides
        IList<OAuth2ResponseMode>? responseModesSupportedOverride =
            openIdConfigurationGetCustomBehaviour?.ResponseModesSupportedResponse;
        if (!(responseModesSupportedOverride is null))
        {
            openIdConfiguration.ResponseModesSupported = responseModesSupportedOverride;
        }

        IList<TokenEndpointAuthMethodOpenIdConfiguration>? tokenEndpointAuthMethodsSupportedOverride =
            openIdConfigurationGetCustomBehaviour?.TokenEndpointAuthMethodsSupportedResponse;
        if (!(tokenEndpointAuthMethodsSupportedOverride is null))
        {
            openIdConfiguration.TokenEndpointAuthMethodsSupported = tokenEndpointAuthMethodsSupportedOverride;
        }

        // Validate OpenID Connect configuration
        IEnumerable<IFluentResponseInfoOrWarningMessage> newNonErrorMessages =
            new OpenBankingOpenIdConfigurationResponseValidator()
                .Validate(openIdConfiguration)
                .ProcessValidationResultsAndRaiseErrors("prefix");

        return (openIdConfiguration, newNonErrorMessages);
    }
}
