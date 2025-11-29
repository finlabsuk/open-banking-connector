// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

/// <summary>
///     Fluent interface methods for updating a VRP consent
/// </summary>
public interface IUpdateVrpConsentContext
{
    /// <summary>
    ///     Migrate VRP consent from v3 to v4 at external (bank) API
    /// </summary>
    /// <param name="request"></param>
    /// <param name="updateParams"></param>
    /// <returns></returns>
    Task<DomesticVrpConsentCreateResponse> UpdateAsync(
        DomesticVrpConsentRequest request,
        ConsentBaseReadParams updateParams);
}
