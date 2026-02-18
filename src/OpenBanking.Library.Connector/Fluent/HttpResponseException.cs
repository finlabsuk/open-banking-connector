// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent;

public enum ProblemDetailsTitle
{
    AuthContextNotFound,
    AuthContextStale
}

/// <summary>
///     Exception exposed via HTTP response
/// </summary>
public class HttpResponseException(
    ProblemDetailsTitle title,
    string detail,
    int statusCode,
    IDictionary<string, object?>? extensions = null) : Exception(detail)
{
    public ProblemDetailsTitle Title { get; } = title;

    public int StatusCode { get; } = statusCode;

    public IDictionary<string, object?>? Extensions { get; } = extensions;
}
