// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

public abstract class ConsentExternalReadParams
{
    public required Guid ConsentId { get; init; }
    public required string? ModifiedBy { get; init; }

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

public class AccountAccessConsentExternalReadParams : ConsentExternalReadParams
{
    /// <summary>
    ///     Account ID associated with external object(s)
    /// </summary>
    public required string? ExternalApiAccountId { get; init; }

    /// <summary>
    ///     Enables pass-through of query parameters to external API (bank) request. This is useful in situations where
    ///     pagination is used and pages are specified via query parameters.
    /// </summary>
    public required string? QueryString { get; init; }
}

public class TransactionsReadParams : AccountAccessConsentExternalReadParams
{
    public required string? ExternalApiStatementId { get; init; }
}

internal interface IAccountAccessConsentExternalRead<TPublicResponse, TReadParams>
    where TReadParams : AccountAccessConsentExternalReadParams
{
    Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ReadAsync(
        TReadParams readParams);
}
