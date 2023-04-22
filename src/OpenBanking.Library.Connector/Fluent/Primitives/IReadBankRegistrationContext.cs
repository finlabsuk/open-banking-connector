// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

/// <summary>
///     Fluent interface methods for Read.
/// </summary>
/// <typeparam name="TPublicResponse"></typeparam>
public interface IReadBankRegistrationContext<TPublicResponse>
    where TPublicResponse : class
{
    /// <summary>
    ///     READ object by ID (includes GETing object from bank API).
    ///     Object will be read from bank and also from local database if it is a Bank Registration or Consent.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="modifiedBy"></param>
    /// <param name="includeExternalApiOperation"></param>
    /// <param name="useRegistrationAccessToken"></param>
    /// <returns></returns>
    Task<TPublicResponse> ReadAsync(
        Guid id,
        string? modifiedBy = null,
        bool? includeExternalApiOperation = null,
        bool? useRegistrationAccessToken = null);
}

internal interface
    IReadBankRegistrationContextInternal<TPublicResponse> : IReadBankRegistrationContext<TPublicResponse>
    where TPublicResponse : class
{
    IObjectRead<TPublicResponse, BankRegistrationReadParams> ReadObject { get; }

    async Task<TPublicResponse> IReadBankRegistrationContext<TPublicResponse>.ReadAsync(
        Guid id,
        string? modifiedBy,
        bool? includeExternalApiOperation,
        bool? useRegistrationAccessToken)
    {
        var readParams = new BankRegistrationReadParams(
            id,
            modifiedBy,
            useRegistrationAccessToken,
            includeExternalApiOperation);
        (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
            await ReadObject.ReadAsync(readParams);

        return response;
    }
}
