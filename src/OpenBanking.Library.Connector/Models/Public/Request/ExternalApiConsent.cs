// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

public class AccessToken
{
    /// <summary>
    ///     Token. Default value is placeholder with expires_in set to zero in order to trigger use
    ///     of refresh token.
    /// </summary>
    public string Token { get; set; } = "expired_token";

    /// <summary>
    ///     Token "expires_in". Default token is placeholder with expires_in set to zero in order to trigger use
    ///     of refresh token.
    /// </summary>
    public int ExpiresIn { get; set; } = 0;

    public string? RefreshToken { get; set; }

    public string? ModifiedBy { get; set; }
}

public class ExternalApiConsent
{
    /// <summary>
    ///     External (bank) API ID, i.e. ID of object at bank. This should be unique between objects created at the
    ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
    /// </summary>
    public string ExternalApiId { get; set; } = null!;

    public AccessToken? AccessToken { get; set; }
}
