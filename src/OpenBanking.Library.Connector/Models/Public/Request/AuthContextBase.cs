// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

public class AuthContextBase : EntityBase
{
    /// <summary>
    ///     Optionally specify OAuth2 state to use instead of one automatically created by Open Banking Connector (using this
    ///     is not recommended).
    /// </summary>
    public string? State { get; init; }

    /// <summary>
    ///     Optionally specify which registered redirect URI to use for this auth context. Defaults to the bank
    ///     registration's/software statement's default for the applicable response_mode.
    /// </summary>
    public string? RedirectUri { get; init; }
}
