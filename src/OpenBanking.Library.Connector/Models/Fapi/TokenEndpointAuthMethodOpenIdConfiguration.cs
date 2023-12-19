// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;

/// <summary>
///     Valid elements for token_endpoint_auth_methods_supported in OpenID Configuration
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum TokenEndpointAuthMethodOpenIdConfiguration
{
    [EnumMember(Value = "client_secret_basic")]
    ClientSecretBasic,

    [EnumMember(Value = "client_secret_post")]
    ClientSecretPost,

    [EnumMember(Value = "client_secret_jwt")]
    ClientSecretJwt,

    [EnumMember(Value = "private_key_jwt")]
    PrivateKeyJwt,

    [EnumMember(Value = "tls_client_auth")]
    TlsClientAuth,

    [EnumMember(Value = "none")]
    None
}
