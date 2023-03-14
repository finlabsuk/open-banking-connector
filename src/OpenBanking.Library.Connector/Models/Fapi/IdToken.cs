// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;

[JsonConverter(typeof(StringEnumConverter))]
public enum JsonWebKeyType
{
    [EnumMember(Value = "EC")]
    Ec,

    [EnumMember(Value = "RSA")]
    Rsa
}

[JsonConverter(typeof(StringEnumConverter))]
public enum JsonWebKeyUse
{
    [EnumMember(Value = "sig")]
    Signing,

    [EnumMember(Value = "tls")]
    Tls,

    [EnumMember(Value = "enc")]
    Encoding
}

[JsonConverter(typeof(StringEnumConverter))]
public enum JsonWebKeyAlg
{
    [EnumMember(Value = "PS256")]
    Ps256,

    [EnumMember(Value = "RS256")]
    Rs256
}

public class JsonWebKey
{
    [JsonProperty("kty", Required = Required.Always)]
    public JsonWebKeyType KeyType { get; set; }

    [JsonProperty("use", Required = Required.Always)]
    public JsonWebKeyUse Use { get; set; }

    [JsonProperty("kid", Required = Required.Always)]
    public string KId { get; set; } = null!;

    [JsonProperty("alg")]
    public JsonWebKeyAlg? Alg { get; set; }

    [JsonProperty("e", Required = Required.Always)]
    public string RsaExponent { get; set; } = null!;

    [JsonProperty("n", Required = Required.Always)]
    public string RsaModulus { get; set; } = null!;
}

public class Jwks
{
    [JsonProperty("keys")]
    public List<JsonWebKey> Keys { get; set; } = null!;
}

public abstract class IdTokenBase
{
    /// <summary>
    ///     Gets or sets unique identifier for the TPP. Implemented as Base62
    ///     encoded GUID
    /// </summary>
    [JsonProperty(PropertyName = "iss", Required = Required.Always)]
    public string Issuer { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the time at which the request was issued by the TPP
    ///     expressed as seconds since 1970-01-01T00:00:00Z as measured in UTC
    /// </summary>
    [JsonProperty(PropertyName = "iat", Required = Required.Always)]
    [JsonConverter(typeof(DateTimeOffsetUnixConverter))]
    public DateTimeOffset Iat { get; set; }

    [JsonProperty(PropertyName = "nbf")]
    [JsonConverter(typeof(DateTimeOffsetNullableUnixConverter))]
    public DateTimeOffset? Nbf { get; set; }

    /// <summary>
    ///     Gets or sets the time at which the request expires expressed as
    ///     seconds since 1970-01-01T00:00:00Z as measured in UTC
    /// </summary>
    [JsonProperty(PropertyName = "exp", Required = Required.Always)]
    [JsonConverter(typeof(DateTimeOffsetUnixConverter))]
    public DateTimeOffset Exp { get; set; }

    [JsonProperty(PropertyName = "sub", Required = Required.Always)]
    public string Subject { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the audience for the request. This should be the
    ///     unique identifier
    ///     for the ASPSP issued by the issuer of the software statement.
    ///     Implemented as Base62 encoded GUID
    /// </summary>
    [JsonProperty(PropertyName = "aud", Required = Required.Always)]
    public string Audience { get; set; } = null!;

    /// <summary>
    ///     Gets or sets unique identifier for the JWT implemented as UUID v4
    /// </summary>
    [JsonProperty(PropertyName = "jti")]
    public string? Jti { get; set; }

    [JsonProperty("nonce", Required = Required.Always)]
    public string Nonce { get; set; } = null!;

    [JsonProperty(PropertyName = "auth_time")]
    public string? AuthTime { get; set; }

    [JsonProperty("openbanking_intent_id", Required = Required.Always)]
    public string ConsentId { get; set; } = null!;

    [JsonProperty("acr", Required = Required.Always)]
    public Acr Acr { get; set; }
}

public class IdTokenAuthEndpoint : IdTokenBase
{
    [JsonProperty("c_hash", Required = Required.Always)]
    public string CodeHash { get; set; } = null!;

    [JsonProperty("s_hash", Required = Required.Always)]
    public string StateHash { get; set; } = null!;
}

public class IdTokenTokenEndpoint : IdTokenBase
{
    [JsonProperty("c_hash")]
    public string? CodeHash { get; set; }

    [JsonProperty("s_hash")]
    public string? StateHash { get; set; }

    [JsonProperty("at_hash")]
    public string? AccessTokenHash { get; set; }
}
