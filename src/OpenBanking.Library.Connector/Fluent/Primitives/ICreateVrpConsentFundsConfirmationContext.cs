// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

/// <summary>
///     Fluent interface methods for Read.
/// </summary>
/// <typeparam name="TPublicResponse"></typeparam>
/// <typeparam name="TPublicRequest"></typeparam>
public interface ICreateVrpConsentFundsConfirmationContext<in TPublicRequest, TPublicResponse>
    where TPublicResponse : class
{
    /// <summary>
    ///     CREATE funds confirmation for consent (includes POSTing object to bank API).
    /// </summary>
    /// <param name="request"></param>
    /// <param name="createParams"></param>
    /// <returns></returns>
    Task<TPublicResponse> CreateFundsConfirmationAsync(
        TPublicRequest request,
        VrpConsentFundsConfirmationCreateParams createParams);
}
