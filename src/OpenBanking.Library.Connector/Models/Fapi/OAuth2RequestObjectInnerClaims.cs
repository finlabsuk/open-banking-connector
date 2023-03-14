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

public class OAuth2RequestObjectInnerClaims
{
    public OAuth2RequestObjectInnerClaims(
        string externalApiConsentId,
        string? consentIdClaimPrefix,
        bool supportsSca)
    {
        string consentIdClaimValue = consentIdClaimPrefix + externalApiConsentId;
        UserInfo = new UserInfoClaims(consentIdClaimValue);
        IdToken = new IdTokenClaims(consentIdClaimValue, supportsSca);
    }

    [JsonProperty("userinfo")]
    public UserInfoClaims UserInfo { get; set; }

    [JsonProperty("id_token")]
    public IdTokenClaims IdToken { get; set; }

    public class StringClaim
    {
        public StringClaim(bool essential, string? value, string[]? values)
        {
            Essential = essential;
            Value = value;
            Values = values;
        }

        [JsonProperty("essential")]
        public bool Essential { get; set; }

        [JsonProperty("value")]
        public string? Value { get; set; }

        [JsonProperty("values")]
        public string[]? Values { get; set; }
    }

    public class AcrClaim
    {
        [JsonProperty("essential")]
        public bool Essential { get; set; }

        [JsonProperty("value")]
        public Acr? Value { get; set; }

        [JsonProperty("values")]
        public IList<Acr>? Values { get; set; }
    }

    public class UserInfoClaims
    {
        public UserInfoClaims(string consentIdClaimValue)
        {
            OpenbankingIntentId = new StringClaim(
                true,
                consentIdClaimValue,
                null);
        }

        [JsonProperty("openbanking_intent_id")]
        public StringClaim OpenbankingIntentId { get; set; }
    }

    public class IdTokenClaims
    {
        public IdTokenClaims(string consentIdClaimValue, bool supportsSca)
        {
            ConsentIdClaim = new StringClaim(
                true,
                consentIdClaimValue,
                null);
            AcrClaim = supportsSca
                ? new AcrClaim
                {
                    Essential = true,
                    Value = null,
                    Values = new List<Acr>
                    {
                        Acr.Ca,
                        Acr.Sca
                    }
                }
                : new AcrClaim
                {
                    Essential = true,
                    Value = Acr.Ca,
                    Values = null
                };
        }

        [JsonProperty("openbanking_intent_id")]
        public StringClaim ConsentIdClaim { get; set; }

        [JsonProperty("acr")]
        public AcrClaim AcrClaim { get; set; }
    }
}
