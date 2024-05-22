// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

/// <summary>
///     Fluent interface methods for Read.
/// </summary>
/// <typeparam name="TPublicResponse"></typeparam>
public interface IReadTransactionsContext<TPublicResponse>
    where TPublicResponse : class
{
    private protected IAccountAccessConsentExternalRead<TPublicResponse, TransactionsReadParams> ReadObject { get; }

    /// <summary>
    ///     READ objects using consent ID (includes GETing objects from bank API).
    ///     Objects will be read from bank database only.
    /// </summary>
    /// <param name="consentId"></param>
    /// <param name="externalApiAccountId"></param>
    /// <param name="externalApiStatementId"></param>
    /// <param name="modifiedBy"></param>
    /// <param name="extraHeaders"></param>
    /// <param name="queryString"></param>
    /// <param name="requestUrlWithoutQuery"></param>
    /// <returns></returns>
    async Task<TPublicResponse> ReadAsync(
        Guid consentId,
        string? externalApiAccountId = null,
        string? externalApiStatementId = null,
        string? modifiedBy = null,
        IEnumerable<HttpHeader>? extraHeaders = null,
        string? queryString = null,
        string? requestUrlWithoutQuery = null)
    {
        var transactionsReadParams = new TransactionsReadParams
        {
            ConsentId = consentId,
            ModifiedBy = modifiedBy,
            ExtraHeaders = extraHeaders,
            PublicRequestUrlWithoutQuery = requestUrlWithoutQuery,
            ExternalApiAccountId = externalApiAccountId,
            QueryString = queryString,
            ExternalApiStatementId = externalApiStatementId
        };
        (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
            await ReadObject.ReadAsync(transactionsReadParams);

        return response;
    }
}
