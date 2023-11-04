// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;

internal class AuthGrantPostRequestProcessor<TRequest> : IPostRequestProcessor<TRequest>
    where TRequest : Dictionary<string, string>
{
    private readonly string _externalApiClientId;
    private readonly string? _externalApiClientSecret;
    private readonly TokenEndpointAuthMethod _tokenEndpointAuthMethod;

    public AuthGrantPostRequestProcessor(
        string externalApiClientId,
        string? externalApiClientSecret,
        TokenEndpointAuthMethod tokenEndpointAuthMethod)
    {
        _externalApiClientId = externalApiClientId;
        _externalApiClientSecret = externalApiClientSecret;
        _tokenEndpointAuthMethod = tokenEndpointAuthMethod;
    }

    (List<HttpHeader> headers, string body, string contentType) IPostRequestProcessor<TRequest>.HttpPostRequestData(
        TRequest variantRequest,
        JsonSerializerSettings? requestJsonSerializerSettings,
        string requestDescription)
    {
        // Assemble headers and body
        var headers = new List<HttpHeader>();
        switch (_tokenEndpointAuthMethod)
        {
            case TokenEndpointAuthMethod.TlsClientAuth:
                variantRequest["client_id"] = _externalApiClientId;
                break;
            case TokenEndpointAuthMethod.ClientSecretBasic:
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
            case TokenEndpointAuthMethod.PrivateKeyJwt:
                break;
            default:
                throw new InvalidOperationException("Found unsupported TokenEndpointAuthMethod");
        }

        string content = variantRequest.ToUrlEncoded();

        return (headers, content, "application/x-www-form-urlencoded");
    }
}
