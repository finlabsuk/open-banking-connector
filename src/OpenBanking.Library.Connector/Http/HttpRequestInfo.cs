// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

public class HttpRequestInfo
{
    public Uri? RequestUri { get; set; }

    public HttpMethod Method { get; set; } = HttpMethod.Get;

    public List<HttpHeader> Headers { get; set; } = new();

    public Func<(HttpContent Content, string ContentForLog)>? GetContent { get; set; }

    public (HttpRequestMessage Message, string? RequestContentForLog) CreateRequestMessage()
    {
        if (RequestUri is null)
        {
            throw new InvalidOperationException("Uri has not been set.");
        }
        var message = new HttpRequestMessage(Method, RequestUri);

        // Set headers
        message.Headers.UserAgent.ParseAdd("OpenBankingConnector");
        if (Headers.Count > 0)
        {
            IEnumerable<IGrouping<string, HttpHeader>> headers = Headers.GroupBy(h => h.Name);
            foreach (IGrouping<string, HttpHeader> headerGroup in headers)
            {
                IEnumerable<string> values1 = headerGroup.Select(h => h.Value);
                message.Headers.Add(headerGroup.Key, values1);
            }
        }

        // Set content
        string? requestContentForLog = null;
        if (GetContent is not null)
        {
            (message.Content, requestContentForLog) = GetContent();
        }

        return (message, requestContentForLog);
    }
}
