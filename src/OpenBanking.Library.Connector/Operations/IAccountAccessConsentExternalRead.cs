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
    public required IEnumerable<HttpHeader>? ExtraHeaders { get; init; }
    public required string? PublicRequestUrlWithoutQuery { get; init; }
}

internal class AccountAccessConsentExternalReadParams : ConsentExternalReadParams
{
    public required string? ExternalApiAccountId { get; init; }
    public required string? QueryString { get; init; }
}

internal class TransactionsReadParams : AccountAccessConsentExternalReadParams
{
    public required string? ExternalApiStatementId { get; init; }
}

internal interface IAccountAccessConsentExternalRead<TPublicResponse, TReadParams>
    where TReadParams : AccountAccessConsentExternalReadParams
{
    Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ReadAsync(
        TReadParams readParams);
}
