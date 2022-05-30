// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

public class ErrorAndLoggingHandler : DelegatingHandler
{
    public ErrorAndLoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        var statusCode = (int) response.StatusCode;
        if (statusCode >= 400)
        {
            throw new ExternalApiHttpErrorException(
                statusCode,
                $"{request.Method}",
                $"{request.RequestUri}",
                await response.Content.ReadAsStringAsync(cancellationToken));
        }

        return response;
    }
}
