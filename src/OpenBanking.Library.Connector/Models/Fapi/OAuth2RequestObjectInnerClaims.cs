// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;

[JsonConverter(typeof(StringEnumConverter))]
public enum Acr
{
    [EnumMember(Value = "urn:openbanking:psd2:ca")]
    Ca,

    [EnumMember(Value = "urn:openbanking:psd2:sca")]
    Sca
}

public class StringClaim
{
    [JsonProperty("essential")]
    public required bool Essential { get; init; }

    [JsonProperty("value")]
    public string? Value { get; init; }

    [JsonProperty("values")]
    public string[]? Values { get; init; }
}

public class AcrClaim
{
    [JsonProperty("essential")]
    public required bool Essential { get; init; }

    [JsonProperty("value")]
    public Acr? Value { get; init; }

    [JsonProperty("values")]
    public IList<Acr>? Values { get; init; }
}

public class UserInfoClaims
{
    [JsonProperty("openbanking_intent_id")]
    public required StringClaim OpenbankingIntentId { get; init; }
}

public class IdTokenClaims
{
    [JsonProperty("openbanking_intent_id")]
    public required StringClaim ConsentIdClaim { get; init; }

    [JsonProperty("acr")]
    public required AcrClaim AcrClaim { get; init; }
}

public class OAuth2RequestObjectInnerClaims
{
    [JsonProperty("userinfo")]
    public required UserInfoClaims UserInfo { get; init; }

    [JsonProperty("id_token")]
    public required IdTokenClaims IdToken { get; init; }
}
