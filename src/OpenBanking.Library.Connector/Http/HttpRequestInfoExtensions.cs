// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http
{
    internal static class HttpRequestInfoExtensions
    {
        public static HttpRequestMessage CreateRequestMessage(this HttpRequestInfo info)
        {
            HttpRequestMessage result = new HttpRequestMessage(
                    method: new HttpMethod(info.Method),
                    requestUri: info.RequestUri.ToString())
                .ApplyAcceptEncoding()
                .ApplyAcceptContentTypes(info)
                .AddHeaders(info)
                .ApplyContent(info)
                .ApplyUserAgent(info);

            return result;
        }

        private static HttpRequestMessage ApplyAcceptEncoding(this HttpRequestMessage request)
        {
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));

            return request;
        }

        private static HttpRequestMessage ApplyAcceptContentTypes(this HttpRequestMessage request, HttpRequestInfo info)
        {
            List<string> acceptableContentTypes = new List<string> { "application/json", "application/jose+jwe" };

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
                HttpContent content = new StringContent(content: info.Content, encoding: Encoding.UTF8);
                string contentType = info.ContentTypes.FirstOrDefault();
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
            if (info.Headers?.Count > 0)
            {
                IEnumerable<IGrouping<string, HttpHeader>> headers = info.Headers.GroupBy(h => h.Name);
                foreach (IGrouping<string, HttpHeader> headerGroup in headers)
                {
                    IEnumerable<string> values = headerGroup.Select(h => h.Value);
                    request.Headers.Add(name: headerGroup.Key, values: values);
                }
            }

            return request;
        }
    }
}
