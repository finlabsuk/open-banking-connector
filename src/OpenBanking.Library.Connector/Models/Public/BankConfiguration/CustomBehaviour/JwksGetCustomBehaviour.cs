// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;

/// <summary>
///     Custom behaviour when accessing Jwks (JSON Web Keys) endpoint ("JwksUri").
///     Custom behaviour is specified by non-null property values and
///     null property values do not change default behaviour.
/// </summary>
public class JwksGetCustomBehaviour
{
    /// <summary>
    ///     Response does not have top-level (root) property. (The top-level property "keys" is expected by default.)
    /// </summary>
    public bool? ResponseHasNoRootProperty { get; set; }
}
