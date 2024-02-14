// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

/// <summary>
///     Abstraction of HttpClient to support replay functionality (ReplayClient). Abstraction level allows HTTP request
///     logging to work with replay but means real HttpClient (and its handlers) not used for replay which is considered
///     network-related stuff.
/// </summary>
public interface IHttpClient : IDisposable
{
    Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken);
}
