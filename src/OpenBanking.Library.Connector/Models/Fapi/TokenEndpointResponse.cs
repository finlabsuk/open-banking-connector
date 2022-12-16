// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi
{
    /// <summary>
    ///     OAuth2 parameters apart from refresh token.
    /// </summary>
    public abstract class TokenEndpointResponseBase
    {
        [JsonProperty("access_token", Required = Required.Always)]
        public string AccessToken { get; set; } = null!;

        [JsonProperty("expires_in", Required = Required.Always)]
        public int ExpiresIn { get; set; }

        [JsonProperty("token_type", Required = Required.Always)]
        public string TokenType { get; set; } = null!;

        public string? Scope { get; set; }
    }

    public class TokenEndpointResponseClientCredentialsGrant : TokenEndpointResponseBase
    {
        /// <summary>
        ///     Allows checking for presence of ID token (not expected)
        /// </summary>
        [JsonProperty("id_token")]
        public string? IdToken { get; set; }
    }

    public class TokenEndpointResponseAuthCodeGrant : TokenEndpointResponseBase
    {
        [JsonProperty("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonProperty("id_token", Required = Required.Always)]
        public string IdToken { get; set; } = null!;
    }

    public class TokenEndpointResponseRefreshTokenGrant : TokenEndpointResponseBase
    {
        [JsonProperty("refresh_token", Required = Required.Always)]
        public string RefreshToken { get; set; } = null!;

        [JsonProperty("id_token")]
        public string? IdToken { get; set; }
    }
}
