// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent;

/// <summary>
///     Exception exposed via HTTP response
/// </summary>
public class HttpResponseException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
