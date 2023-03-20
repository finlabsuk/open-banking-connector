// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Http.Headers;
using System.Text;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

internal static class HttpRequestInfoExtensions
{
    public static HttpRequestMessage CreateRequestMessage(this HttpRequestInfo info)
    {
        HttpRequestMessage result = new HttpRequestMessage(
                new HttpMethod(info.Method),
                info.RequestUri.ToString())
            .ApplyAcceptContentTypes(info)
            .AddHeaders(info)
            .ApplyContent(info)
            .ApplyUserAgent(info);

        return result;
    }

    private static HttpRequestMessage ApplyAcceptContentTypes(this HttpRequestMessage request, HttpRequestInfo info)
    {
        var acceptableContentTypes = new List<string> { "application/json" };

        foreach (string contentType in acceptableContentTypes)
        {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
        }

        return request;
    }


    private static HttpRequestMessage ApplyUserAgent(this HttpRequestMessage request, HttpRequestInfo info)
    {
        if (!string.IsNullOrEmpty(info.UserAgent))
        {
            request.Headers.UserAgent.ParseAdd(info.UserAgent);
        }

        return request;
    }

    private static HttpRequestMessage ApplyContent(this HttpRequestMessage request, HttpRequestInfo info)
    {
        if (!string.IsNullOrWhiteSpace(info.Content))
        {
            HttpContent content = new StringContent(info.Content, Encoding.UTF8);
            string? contentType = info.ContentTypes.FirstOrDefault();
            if (contentType != null)
            {
                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue(contentType);
            }

            request.Content = content;
        }

        return request;
    }

    private static HttpRequestMessage AddHeaders(this HttpRequestMessage request, HttpRequestInfo info)
    {
        if (info.Headers.Count > 0)
        {
            IEnumerable<IGrouping<string, HttpHeader>> headers = info.Headers.GroupBy(h => h.Name);
            foreach (IGrouping<string, HttpHeader> headerGroup in headers)
            {
                IEnumerable<string> values = headerGroup.Select(h => h.Value);
                request.Headers.Add(headerGroup.Key, values);
            }
        }

        return request;
    }
}
