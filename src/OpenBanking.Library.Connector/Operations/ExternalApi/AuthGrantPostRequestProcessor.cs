// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;

internal class AuthGrantPostRequestProcessor<TRequest> : IPostRequestProcessor<TRequest>
    where TRequest : Dictionary<string, string>
{
    private readonly string _externalApiClientId;
    private readonly string? _externalApiClientSecret;
    private readonly TokenEndpointAuthMethodSupportedValues _tokenEndpointAuthMethod;

    public AuthGrantPostRequestProcessor(
        string externalApiClientId,
        string? externalApiClientSecret,
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod)
    {
        _externalApiClientId = externalApiClientId;
        _externalApiClientSecret = externalApiClientSecret;
        _tokenEndpointAuthMethod = tokenEndpointAuthMethod;
    }

    public async Task<(TResponse response, string? xFapiInteractionId)> PostAsync<TResponse>(
        Uri uri,
        IEnumerable<HttpHeader>? extraHeaders,
        TRequest request,
        TppReportingRequestInfo? tppReportingRequestInfo,
        JsonSerializerSettings? requestJsonSerializerSettings,
        JsonSerializerSettings? responseJsonSerializerSettings,
        IApiClient apiClient)
        where TResponse : class
    {
        // Assemble headers and body
        var headers = new List<HttpHeader>();
        switch (_tokenEndpointAuthMethod)
        {
            case TokenEndpointAuthMethodSupportedValues.TlsClientAuth:
                request["client_id"] = _externalApiClientId;
                break;
            case TokenEndpointAuthMethodSupportedValues.ClientSecretBasic:
            {
                string clientSecret =
                    _externalApiClientSecret ??
                    throw new InvalidOperationException("No client secret available.");
                string authString = _externalApiClientId + ":" + clientSecret;
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(authString);
                string authHeader = "Basic " + Convert.ToBase64String(plainTextBytes);
                headers.Add(new HttpHeader("Authorization", authHeader));
                break;
            }
            case TokenEndpointAuthMethodSupportedValues.PrivateKeyJwt:
                break;
            case TokenEndpointAuthMethodSupportedValues.ClientSecretPost:
                break;
            default:
                throw new InvalidOperationException("Found unsupported TokenEndpointAuthMethod");
        }
        if (extraHeaders is not null)
        {
            foreach (HttpHeader header in extraHeaders)
            {
                headers.Add(header);
            }
        }
        string content = request.ToUrlParameterString();
        var contentType = "application/x-www-form-urlencoded";

        // Send request
        (TResponse response, string? xFapiInteractionId) = await new HttpRequestBuilder()
            .SetMethod(HttpMethod.Post)
            .SetUri(uri)
            .SetHeaders(headers)
            .SetTextContent(content, contentType)
            .SendExpectingJsonResponseAsync<TResponse>(
                apiClient,
                tppReportingRequestInfo,
                responseJsonSerializerSettings);

        return (response, xFapiInteractionId);
    }
}
