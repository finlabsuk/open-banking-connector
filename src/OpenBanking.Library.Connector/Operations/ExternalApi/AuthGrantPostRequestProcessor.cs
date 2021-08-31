// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

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
            string requestDescription)
        {
            // Assemble headers and body
            List<HttpHeader> headers = new List<HttpHeader>();
            switch (_bankRegistration.BankApiResponse.Data.TokenEndpointAuthMethod)
            {
                case ClientRegistrationModelsPublic.OBRegistrationProperties1tokenEndpointAuthMethodEnum.TlsClientAuth:
                    variantRequest["client_id"] = _bankRegistration.BankApiResponse.Data.ClientId;
                    break;
                case ClientRegistrationModelsPublic.OBRegistrationProperties1tokenEndpointAuthMethodEnum
                    .ClientSecretBasic:
                {
                    _bankRegistration.BankApiResponse.Data.ClientSecret.InvalidOpOnNull("No client secret available.");
                    string authString = _bankRegistration.BankApiResponse.Data.ClientId + ":" +
                                        _bankRegistration.BankApiResponse.Data.ClientSecret;
                    byte[] plainTextBytes = Encoding.UTF8.GetBytes(authString);
                    string authHeader = "Basic " + Convert.ToBase64String(plainTextBytes);
                    headers.Add(new HttpHeader("Authorization", authHeader));
                    break;
                }
                default:
                    throw new InvalidOperationException("Found unsupported TokenEndpointAuthMethod");
            }

            string content = variantRequest.ToUrlEncoded();

            return (headers, content, "application/x-www-form-urlencoded");
        }
    }
}
