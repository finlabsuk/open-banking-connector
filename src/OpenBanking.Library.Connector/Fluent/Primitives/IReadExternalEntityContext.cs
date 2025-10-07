// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Http;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

public class ExternalEntityReadParams
{
    public required string ExternalApiId { get; init; }

    public required Guid BankRegistrationId { get; init; }

    public required bool? UseV4ExternalApi { get; init; }

    /// <summary>
    ///     Enables pass-through of supported headers such as "x-fapi-customer-ip-address" to external API (bank) request.
    /// </summary>
    public required IEnumerable<HttpHeader>? ExtraHeaders { get; init; }

    /// <summary>
    ///     URL of request which enables link URLs in external API response to be transformed into links that work with Open
    ///     Banking Connector.
    /// </summary>
    public required string? PublicRequestUrlWithoutQuery { get; init; }
}

/// <summary>
///     Fluent interface methods for Read.
/// </summary>
/// <typeparam name="TPublicResponse"></typeparam>
/// <typeparam name="TReadParams"></typeparam>
public interface IReadExternalEntityContext<TPublicResponse, in TReadParams>
    where TPublicResponse : class
    where TReadParams : ExternalEntityReadParams
{
    /// <summary>
    ///     READ objects using consent ID (includes GETing objects from bank API).
    ///     Objects will be read from bank database only.
    /// </summary>
    /// <param name="readParams"></param>
    /// <returns></returns>
    Task<TPublicResponse> ReadAsync(
        TReadParams readParams);
}
