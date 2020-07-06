// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http
{
    internal class HttpMessageHandlerFactory
    {
        public HttpMessageHandler Create(HttpRequestInfo value)
        {
            value.ArgNotNull(nameof(value));

            HttpClientHandler clientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };

            ApplyAuthentication(value: value, clientHandler: clientHandler);

            ApplyCertificates(value: value, clientHandler: clientHandler);

            ApplyServerCertificateValidator(value: value, clientHandler: clientHandler);

            ApplyProxy(value: value, clientHandler: clientHandler);

            ApplyRedirects(value: value, clientHandler: clientHandler);

            ApplyCookies(value: value, clientHandler: clientHandler);


            return clientHandler;
        }

        private void ApplyAuthentication(HttpRequestInfo value, HttpClientHandler clientHandler)
        {
            if (value.UseDefaultCredentials.HasValue)
            {
                clientHandler.UseDefaultCredentials = value.UseDefaultCredentials.Value;
            }

            if (value.PreAuthenticate.HasValue)
            {
                clientHandler.PreAuthenticate = value.PreAuthenticate.Value;
            }

            if (value.Credentials != null)
            {
                clientHandler.Credentials = value.Credentials;
            }
        }

        private void ApplyCookies(HttpRequestInfo value, HttpClientHandler clientHandler)
        {
            if (value.Cookies?.Count > 0)
            {
                foreach (Cookie cookie in value.Cookies.Where(c => !string.IsNullOrWhiteSpace(c.Domain)))
                {
                    clientHandler.CookieContainer.Add(cookie);
                }
            }
        }

        private void ApplyRedirects(HttpRequestInfo value, HttpClientHandler clientHandler)
        {
            if (value.MaxRedirects > 0)
            {
                clientHandler.AllowAutoRedirect = true;
                clientHandler.MaxAutomaticRedirections = value.MaxRedirects;
            }
            else
            {
                clientHandler.AllowAutoRedirect = false;
            }
        }

        private void ApplyProxy(HttpRequestInfo value, HttpClientHandler clientHandler)
        {
            if (value.Proxy != null)
            {
                clientHandler.Proxy = value.Proxy;
                clientHandler.UseProxy = true;
            }
        }

        private void ApplyCertificates(HttpRequestInfo value, HttpClientHandler clientHandler)
        {
            if (value.Certificates?.Count > 0)
            {
                clientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                clientHandler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11;

                foreach (X509Certificate certificate in value.Certificates)
                {
                    clientHandler.ClientCertificates.Add(certificate);
                }
            }
        }

        private void ApplyServerCertificateValidator(HttpRequestInfo value, HttpClientHandler clientHandler)
        {
            if (value.ServerCertificateValidator != null)
            {
                clientHandler.ServerCertificateCustomValidationCallback = value.ServerCertificateValidator.IsOk;
            }
        }
    }
}
