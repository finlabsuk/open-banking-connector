// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;

namespace FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;

public class HttpTraceInfo : TraceInfo
{
    public HttpTraceInfo(string message, Uri url) : base(message)
    {
        Url = url;
    }

    public HttpTraceInfo(string message, Uri url, HttpStatusCode statusCode) : base(message)
    {
        Url = url;
        StatusCode = statusCode;
    }

    public Uri Url { get; }

    public HttpStatusCode? StatusCode { get; }
}
