// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Http;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

public class ConsentExternalCreateParams
{
    public required IEnumerable<HttpHeader>? ExtraHeaders { get; init; }

    public required string? PublicRequestUrlWithoutQuery { get; init; }
}

/// <summary>
///     Fluent interface methods for Create.
/// </summary>
/// <typeparam name="TPublicRequest"></typeparam>
/// <typeparam name="TPublicResponse"></typeparam>
/// <typeparam name="TCreateParams"></typeparam>
public interface ICreateExternalEntityContext<in TPublicRequest, TPublicResponse, in TCreateParams>
    where TPublicResponse : class
    where TCreateParams : ConsentExternalCreateParams
{
    /// <summary>
    ///     CREATE object (includes POSTing object to bank API).
    ///     Object will be created at bank and also in local database if it is a Bank Registration or Consent.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="consentExternalCreateParams"></param>
    /// <returns></returns>
    Task<TPublicResponse> CreateAsync(
        TPublicRequest request,
        TCreateParams consentExternalCreateParams);
}
