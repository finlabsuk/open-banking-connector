// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http
{
    public interface IHttpRequestBuilder
    {
        IHttpRequestBuilder SetUri(Uri value);

        IHttpRequestBuilder SetUri(string value);

        IHttpRequestBuilder SetUseDefaultCredentials(bool value);

        IHttpRequestBuilder SetCredentials(ICredentials value);

        IHttpRequestBuilder SetPreAuthenticate(bool value);

        IHttpRequestBuilder SetMethod(HttpMethod method);

        IHttpRequestBuilder SetHeaders(IEnumerable<HttpHeader> values);

        IHttpRequestBuilder SetCookies(IEnumerable<Cookie> values);

        IHttpRequestBuilder SetUserAgent(string value);

        IHttpRequestBuilder SetClientCertificate(X509Certificate2 certificate);

        IHttpRequestBuilder SetClientCertificates(IEnumerable<X509Certificate> certificates);

        IHttpRequestBuilder SetServerCertificateValidator(IServerCertificateValidator validator);

        IHttpRequestBuilder SetTimeout(TimeSpan value);

        IHttpRequestBuilder SetReadWriteTimeout(TimeSpan value);

        IHttpRequestBuilder SetMaxRedirects(int value);

        IHttpRequestBuilder SetProxy(IWebProxy value);

        IHttpRequestBuilder SetContent(string content);

        IHttpRequestBuilder SetContentType(string contentType);

        HttpRequestMessage Create();

        HttpMessageHandler CreateMessageHandler();
    }
}
