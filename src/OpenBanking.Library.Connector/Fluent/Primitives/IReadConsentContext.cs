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
public interface IReadConsentContext<TPublicResponse>
    where TPublicResponse : class
{
    private protected IObjectRead<TPublicResponse, ConsentReadParams> ReadObject { get; }

    /// <summary>
    ///     READ object by ID (includes GETing object from bank API).
    ///     Object will be read from bank and also from local database if it is a Bank Registration or Consent.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="modifiedBy"></param>
    /// <param name="extraHeaders"></param>
    /// <param name="includeExternalApiOperation"></param>
    /// <param name="publicRequestUrlWithoutQuery"></param>
    /// <returns></returns>
    async Task<TPublicResponse> ReadAsync(
        Guid id,
        string? modifiedBy = null,
        IEnumerable<HttpHeader>? extraHeaders = null,
        bool includeExternalApiOperation = true,
        string? publicRequestUrlWithoutQuery = null)
    {
        var readParams = new ConsentReadParams
        {
            Id = id,
            ModifiedBy = modifiedBy,
            ExtraHeaders = extraHeaders,
            PublicRequestUrlWithoutQuery = publicRequestUrlWithoutQuery,
            IncludeExternalApiOperation = includeExternalApiOperation
        };
        (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
            await ReadObject.ReadAsync(readParams);

        return response;
    }
}
