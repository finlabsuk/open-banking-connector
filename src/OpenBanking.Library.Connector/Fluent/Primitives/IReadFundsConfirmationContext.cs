// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

/// <summary>
///     Fluent interface methods for Read.
/// </summary>
/// <typeparam name="TPublicResponse"></typeparam>
/// <typeparam name="TReadParams"></typeparam>
public interface IReadFundsConfirmationContext<TPublicResponse, TReadParams>
    where TPublicResponse : class
    where TReadParams : ConsentBaseReadParams
{
    /// <summary>
    ///     READ funds confirmation by ID (includes GETing object from bank API).
    /// </summary>
    /// <returns></returns>
    Task<TPublicResponse> ReadFundsConfirmationAsync(
        TReadParams readParams);
}
