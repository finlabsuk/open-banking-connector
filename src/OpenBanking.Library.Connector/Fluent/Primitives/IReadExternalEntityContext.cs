// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

public class ConsentExternalEntityReadParams : ConsentExternalReadParams
{
    public required string ExternalApiId { get; init; }
}

/// <summary>
///     Fluent interface methods for Read.
/// </summary>
/// <typeparam name="TPublicResponse"></typeparam>
/// <typeparam name="TReadParams"></typeparam>
public interface IReadExternalEntityContext<TPublicResponse, in TReadParams>
    where TPublicResponse : class
    where TReadParams : ConsentExternalEntityReadParams
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
