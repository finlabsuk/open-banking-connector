﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Token endpoint auth method.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TokenEndpointAuthMethodEnum
    {
        [EnumMember(Value = "client_secret_basic")]
        ClientSecretBasic,

        [EnumMember(Value = "client_secret_jwt")]
        ClientSecretJwt,

        [EnumMember(Value = "private_key_jwt")]
        PrivateKeyJwt,

        [EnumMember(Value = "tls_client_auth")]
        TlsClientAuth
    }


    [Owned]
    public class OpenIdConfiguration
    {
        [JsonProperty("issuer")]
        public string Issuer { get; set; } = null!;

        [JsonProperty("response_types_supported")]
        public IList<string> ResponseTypesSupported { get; set; } = null!;

        [JsonProperty("scopes_supported")]
        public IList<string> ScopesSupported { get; set; } = null!;

        [JsonProperty("response_modes_supported")]
        public IList<string> ResponseModesSupported { get; set; } = null!;

        [JsonProperty("token_endpoint")]
        public string TokenEndpoint { get; set; } = null!;

        [JsonProperty("authorization_endpoint")]
        public string AuthorizationEndpoint { get; set; } = null!;

        [JsonProperty("registration_endpoint")]
        public string RegistrationEndpoint { get; set; } = null!;

        [JsonProperty("token_endpoint_auth_methods_supported")]
        public IList<TokenEndpointAuthMethodEnum> TokenEndpointAuthMethodsSupported { get; set; } =
            new List<TokenEndpointAuthMethodEnum>();
    }
}
