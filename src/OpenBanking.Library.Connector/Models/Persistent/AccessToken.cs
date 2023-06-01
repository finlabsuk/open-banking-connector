// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;

/// <summary>
///     Access token used for API access
/// </summary>
public class AccessToken
{
    public AccessToken(string token, int expiresIn)
    {
        Token = token;
        ExpiresIn = expiresIn;
    }

    /// <summary>
    ///     Token.
    /// </summary>
    public string Token { get; }

    /// <summary>
    ///     Token "expires_in".
    /// </summary>
    public int ExpiresIn { get; }

    public static string GetPlainText(AccessToken accessToken) => JsonConvert.SerializeObject(
        accessToken,
        Formatting.None,
        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
}
