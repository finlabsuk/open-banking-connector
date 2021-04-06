// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using FinnovationLabs.OpenBanking.Library.Connector.Http;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Http
{
    public class HttpClientHandlerFactoryTests
    {
        [HttpRequestInfoProperty]
        public bool Create_DecompressionAlwaysOn(HttpRequestInfo value)
        {
            var handler = new HttpMessageHandlerFactory().Create(value) as HttpClientHandler;

            return ((handler!.AutomaticDecompression & DecompressionMethods.Deflate) == DecompressionMethods.Deflate) |
                   ((handler!.AutomaticDecompression & DecompressionMethods.GZip) == DecompressionMethods.GZip);
        }


        [HttpRequestInfoProperty]
        public bool Create_Proxy_PassedThrough(HttpRequestInfo info, IWebProxy? value)
        {
            info.Proxy = value;

            var handler = new HttpMessageHandlerFactory().Create(info) as HttpClientHandler;

            return value != null
                ? handler!.UseProxy && ReferenceEquals(handler.Proxy, value)
                : !handler!.UseProxy && ReferenceEquals(handler.Proxy, null);
        }


        [HttpRequestInfoProperty]
        public bool Create_Redirects_PassedThrough(HttpRequestInfo info, int value)
        {
            info.MaxRedirects = value;

            var handler = new HttpMessageHandlerFactory().Create(info) as HttpClientHandler;

            return value > 0
                ? handler!.AllowAutoRedirect && handler.MaxAutomaticRedirections == value
                : !handler!.AllowAutoRedirect;
        }

        [HttpRequestInfoProperty]
        public bool Create_Certificates_PassedThrough(HttpRequestInfo info, List<X509Certificate> values)
        {
            info.Certificates = values;

            var handler = new HttpMessageHandlerFactory().Create(info) as HttpClientHandler;


            X509Certificate[] certs = new X509Certificate[values.Count];
            handler!.ClientCertificates.CopyTo(certs, 0);

            return values.SequenceEqual(certs);
        }
    }
}
