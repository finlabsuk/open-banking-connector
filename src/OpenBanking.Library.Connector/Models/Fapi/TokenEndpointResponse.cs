// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;

/// <summary>
///     Token endpoint response parameters for OAuth2 and Open ID Connect.
/// </summary>
public class TokenEndpointResponse
{
    [JsonProperty("access_token", Required = Required.Always)]
    public string AccessToken { get; set; } = null!;

    [JsonProperty("expires_in", Required = Required.Always)]
    public int ExpiresIn { get; set; }

    [JsonProperty("token_type", Required = Required.Always)]
    public string TokenType { get; set; } = null!;

    /// <summary>
    ///     Note that this parameter, when null, will be updated to match "request" scope where relevant.
    /// </summary>
    [JsonProperty("scope")]
    public string? Scope { get; set; }

    [JsonProperty("refresh_token")]
    public string? RefreshToken { get; set; }

    [JsonProperty("id_token")]
    public string? IdToken { get; set; }
}
