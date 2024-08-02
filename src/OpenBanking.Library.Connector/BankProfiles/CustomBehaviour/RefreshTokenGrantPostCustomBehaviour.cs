// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;

public class RefreshTokenGrantPostCustomBehaviour : GrantPostCustomBehaviour
{
    public bool? IdTokenMayBeAbsent { get; set; }

    public IdTokenProcessingCustomBehaviour? IdTokenProcessingCustomBehaviour { get; set; }

    /// <summary>
    ///     Use to specify custom scope.
    /// </summary>
    public string? Scope { get; init; }
}
