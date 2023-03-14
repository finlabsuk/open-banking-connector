// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

public class HttpRequestBuilder : IHttpRequestBuilder
{
    private readonly HttpMessageHandlerFactory _messageHandlerFactory;

    public HttpRequestBuilder()
    {
        _messageHandlerFactory = new HttpMessageHandlerFactory();
    }

    internal HttpRequestInfo RequestInfo { get; } = new();

    public IHttpRequestBuilder SetUri(Uri value)
    {
        RequestInfo.RequestUri = value.ArgNotNull(nameof(value));

        return this;
    }

    public IHttpRequestBuilder SetUri(string value) => SetUri(new Uri(value));

    public IHttpRequestBuilder SetUseDefaultCredentials(bool value)
    {
        RequestInfo.UseDefaultCredentials = value;

        return this;
    }

    public IHttpRequestBuilder SetCredentials(ICredentials value)
    {
        RequestInfo.Credentials = value.ArgNotNull(nameof(value));

        return this;
    }

    public IHttpRequestBuilder SetPreAuthenticate(bool value)
    {
        RequestInfo.PreAuthenticate = value;

        return this;
    }

    public IHttpRequestBuilder SetMethod(HttpMethod method)
    {
        RequestInfo.Method = method.ArgNotNull(nameof(method)).Method;

        return this;
    }

    public IHttpRequestBuilder SetHeaders(IEnumerable<HttpHeader> values)
    {
        RequestInfo.Headers = values.ArgNotNull(nameof(values)).ToList();

        return this;
    }

    public IHttpRequestBuilder SetCookies(IEnumerable<Cookie> values)
    {
        RequestInfo.Cookies = values.ArgNotNull(nameof(values)).ToList();

        return this;
    }

    public IHttpRequestBuilder SetUserAgent(string value)
    {
        RequestInfo.UserAgent = value.ArgNotNull(nameof(value));
        return this;
    }

    public IHttpRequestBuilder SetClientCertificate(X509Certificate2 certificate)
    {
        return SetClientCertificates(new[] { certificate.ArgNotNull(nameof(certificate)) });
    }

    public IHttpRequestBuilder SetClientCertificates(IEnumerable<X509Certificate> certificates)
    {
        RequestInfo.Certificates = certificates.ArgNotNull(nameof(certificates)).ToList();

        return this;
    }

    public IHttpRequestBuilder SetServerCertificateValidator(IServerCertificateValidator validator)
    {
        RequestInfo.ServerCertificateValidator = validator;

        return this;
    }

    public IHttpRequestBuilder SetProxy(IWebProxy value)
    {
        RequestInfo.Proxy = value.ArgNotNull(nameof(value));

        return this;
    }

    public IHttpRequestBuilder SetContent(string content)
    {
        RequestInfo.Content = content;

        return this;
    }

    public IHttpRequestBuilder SetContentType(string contentType)
    {
        RequestInfo.ContentTypes.Add(contentType);

        return this;
    }

    public IHttpRequestBuilder SetTimeout(TimeSpan value)
    {
        RequestInfo.Timeout = value;

        return this;
    }

    public IHttpRequestBuilder SetReadWriteTimeout(TimeSpan value)
    {
        RequestInfo.ReadWriteTimeout = value;

        return this;
    }

    public IHttpRequestBuilder SetMaxRedirects(int value)
    {
        RequestInfo.MaxRedirects = value;

        return this;
    }

    public HttpMessageHandler CreateMessageHandler() => _messageHandlerFactory.Create(RequestInfo);

    public HttpRequestMessage Create() => RequestInfo.CreateRequestMessage();
}
