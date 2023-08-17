// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

public class LoggingHandler : DelegatingHandler
{
    public LoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken) =>
        await base.SendAsync(request, cancellationToken);
}
