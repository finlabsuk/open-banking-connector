// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi
{
    internal class AuthGrantPostRequestProcessor<TRequest> : IPostRequestProcessor<TRequest>
        where TRequest : Dictionary<string, string>
    {
        private readonly BankRegistration _bankRegistration;

        public AuthGrantPostRequestProcessor(BankRegistration bankRegistration)
        {
            _bankRegistration = bankRegistration;
        }

        (List<HttpHeader> headers, string body, string contentType) IPostRequestProcessor<TRequest>.HttpPostRequestData(
            TRequest variantRequest,
            JsonSerializerSettings? requestJsonSerializerSettings,
            string requestDescription)
        {
            // Assemble headers and body
            var headers = new List<HttpHeader>();
            switch (_bankRegistration.TokenEndpointAuthMethod)
            {
                case TokenEndpointAuthMethodEnum.TlsClientAuth:
                    variantRequest["client_id"] = _bankRegistration.ExternalApiId;
                    break;
                case TokenEndpointAuthMethodEnum.ClientSecretBasic:
                {
                    string clientSecret =
                        _bankRegistration.ExternalApiSecret ??
                        throw new InvalidOperationException("No client secret available.");
                    string authString = _bankRegistration.ExternalApiId + ":" + clientSecret;
                    byte[] plainTextBytes = Encoding.UTF8.GetBytes(authString);
                    string authHeader = "Basic " + Convert.ToBase64String(plainTextBytes);
                    headers.Add(new HttpHeader("Authorization", authHeader));
                    break;
                }
                case TokenEndpointAuthMethodEnum.PrivateKeyJwt:
                    break;
                default:
                    throw new InvalidOperationException("Found unsupported TokenEndpointAuthMethod");
            }

            string content = variantRequest.ToUrlEncoded();

            return (headers, content, "application/x-www-form-urlencoded");
        }
    }
}
