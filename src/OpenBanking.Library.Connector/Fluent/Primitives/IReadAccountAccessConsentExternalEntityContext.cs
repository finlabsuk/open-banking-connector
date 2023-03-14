// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

/// <summary>
///     Fluent interface methods for Read.
/// </summary>
/// <typeparam name="TPublicResponse"></typeparam>
public interface IReadAccountAccessConsentExternalEntityContext<TPublicResponse>
    where TPublicResponse : class
{
    /// <summary>
    ///     READ objects using consent ID (includes GETing objects from bank API).
    ///     Objects will be read from bank database only.
    /// </summary>
    /// <param name="consentId"></param>
    /// <param name="externalApiAccountId"></param>
    /// <param name="modifiedBy"></param>
    /// <param name="queryString"></param>
    /// <param name="requestUrlWithoutQuery"></param>
    /// <returns></returns>
    Task<TPublicResponse> ReadAsync(
        Guid consentId,
        string? externalApiAccountId = null,
        string? modifiedBy = null,
        string? queryString = null,
        string? requestUrlWithoutQuery = null);
}

internal interface
    IReadAccountAccessConsentExternalEntityContextInternal<TPublicResponse> :
        IReadAccountAccessConsentExternalEntityContext<TPublicResponse>
    where TPublicResponse : class
{
    IAccountAccessConsentExternalRead<TPublicResponse, ExternalEntityReadParams> ReadObject { get; }

    async Task<TPublicResponse> IReadAccountAccessConsentExternalEntityContext<TPublicResponse>.ReadAsync(
        Guid consentId,
        string? externalApiAccountId,
        string? modifiedBy,
        string? queryString,
        string? requestUrlWithoutQuery)
    {
        var externalEntityReadParams = new ExternalEntityReadParams(
            consentId,
            modifiedBy,
            externalApiAccountId,
            requestUrlWithoutQuery,
            queryString);
        (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
            await ReadObject.ReadAsync(externalEntityReadParams);

        return response;
    }
}
