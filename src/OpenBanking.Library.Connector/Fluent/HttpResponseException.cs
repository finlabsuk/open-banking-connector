// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent;

public enum ServerErrorType
{
    AuthContextNotFound,
    AuthContextStale,
    ConsentNotFound,
    ConsentFromRequestNotFound,
    IdTokenValidationError,
    ExternalApiHttpRequestIoError,
    ExternalApiHttpRequestTimeout,
    ExternalApiHttpRequestFailure
}

public abstract record ServerError
{
    public abstract ServerErrorType ServerErrorType { get; }

    public abstract string Title { get; }

    public abstract string Detail { get; }

    public abstract int StatusCode { get; }

    public virtual IReadOnlyDictionary<string, object?> Extensions =>
        ImmutableDictionary<string, object?>.Empty;
}

/// <summary>
///     Exception exposed via HTTP response
/// </summary>
public class HttpResponseException(ServerError serverError)
    : Exception(serverError.Detail)
{
    public ServerError ServerError { get; } = serverError;
}
