// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.ObApi.Base.Json;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi
{
    public class OAuth2RequestObjectClaims
    {
        /// <summary>
        /// Gets or sets unique identifier for the TPP. Implemented as Base62
        /// encoded GUID
        /// </summary>
        [JsonProperty(PropertyName = "iss")]
        public string Iss { get; set; } = null!;

        /// <summary>
        /// Gets or sets the time at which the request was issued by the TPP
        /// expressed as seconds since 1970-01-01T00:00:00Z as measured in UTC
        /// </summary>
        [JsonProperty(PropertyName = "iat")]
        [JsonConverter(typeof(DateTimeOffsetUnixConverter))]
        public DateTimeOffset Iat { get; set; }
        
        /// <summary>
        /// Gets or sets the time at which the request expires expressed as
        /// seconds since 1970-01-01T00:00:00Z as measured in UTC
        /// </summary>
        [JsonProperty(PropertyName = "exp")]
        [JsonConverter(typeof(DateTimeOffsetUnixConverter))]
        public DateTimeOffset Exp { get; set; }

        /// <summary>
        /// Gets or sets the audience for the request. This should be the
        /// unique identifier
        /// for the ASPSP issued by the issuer of the software statement.
        /// Implemented as Base62 encoded GUID
        ///
        /// </summary>
        [JsonProperty(PropertyName = "aud")]
        public string Aud { get; set; } = null!;

        /// <summary>
        /// Gets or sets unique identifier for the JWT implemented as UUID v4
        /// </summary>
        [JsonProperty(PropertyName = "jti")]
        public string Jti { get; set; } = null!;

        [JsonProperty("response_type")]
        public string ResponseType { get; set; } = null!;

        [JsonProperty("client_id")]
        public string ClientId { get; set; } = null!;

        [JsonProperty("redirect_uri")]
        public string RedirectUri { get; set; } = null!;

        [JsonProperty("scope")]
        public string Scope { get; set; } = null!;

        [JsonProperty("max_age")]
        public int MaxAge { get; set; }

        [JsonProperty("claims")]
        public OAuth2RequestObjectInnerClaims Claims { get; set; } = null!;

        [JsonProperty("nonce")]
        public string Nonce { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("state")]
        public string State { get; set; } = Guid.NewGuid().ToString();
    }
}
