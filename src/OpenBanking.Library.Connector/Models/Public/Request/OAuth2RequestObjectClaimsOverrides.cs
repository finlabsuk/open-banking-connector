// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    /// <summary>
    ///     Class used to specify overrides to default settings for request object claims.
    ///     Default property values do not change anything so set properties to alter client
    ///     registration claims.
    /// </summary>
    public class OAuth2RequestObjectClaimsOverrides
    {
        [JsonProperty("aud")]
        public string? Audience { get; set; }
    }
}
