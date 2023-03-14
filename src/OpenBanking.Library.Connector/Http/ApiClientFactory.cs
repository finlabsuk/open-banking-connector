// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

/// <summary>
///     Create a singleton ApiClient. For test purposes only
/// </summary>
internal static class ApiClientFactory
{
    private static readonly object Lock = new();
    private static readonly Lazy<HttpClient> HttpClient = CreateHttpClient();
    private static HttpMessageHandler? _handler;

    public static IApiClient CreateApiClient(IInstrumentationClient instrumentation, HttpMessageHandler? handler)
    {
        if (handler != null)
        {
            lock (Lock)
            {
                _handler ??= handler;
            }
        }

        return new ApiClient(instrumentation, HttpClient.Value);
    }

    private static Lazy<HttpClient> CreateHttpClient()
    {
        return new Lazy<HttpClient>(
            () =>
            {
                lock (Lock)
                {
                    return _handler is null
                        ? new HttpClient()
                        : new HttpClient(_handler);
                }
            });
    }
}
