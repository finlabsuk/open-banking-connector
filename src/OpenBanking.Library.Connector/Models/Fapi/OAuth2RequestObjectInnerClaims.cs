// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi
{
    public class OAuth2RequestObjectInnerClaims
    {
        public OAuth2RequestObjectInnerClaims(string externalApiConsentId, string? consentIdClaimPrefix)
        {
            string consentIdClaimValue = consentIdClaimPrefix + externalApiConsentId;
            UserInfo = new UserInfoClaims(consentIdClaimValue);
            IdToken = new IdTokenClaims(consentIdClaimValue);
        }

        [JsonProperty("userinfo")]
        public UserInfoClaims UserInfo { get; set; }

        [JsonProperty("id_token")]
        public IdTokenClaims IdToken { get; set; }

        public class IndividualClaim
        {
            public IndividualClaim(bool? essential, string? value, string[]? values)
            {
                Essential = essential;
                Value = value;
                Values = values;
            }

            [JsonProperty("essential")]
            public bool? Essential { get; set; }

            [JsonProperty("value")]
            public string? Value { get; set; }

            [JsonProperty("values")]
            public string[]? Values { get; set; }
        }

        public class UserInfoClaims
        {
            public UserInfoClaims(string consentIdClaimValue)
            {
                OpenbankingIntentId = new IndividualClaim(
                    true,
                    consentIdClaimValue,
                    null);
            }

            [JsonProperty("openbanking_intent_id")]
            public IndividualClaim OpenbankingIntentId { get; set; }
        }

        public class IdTokenClaims
        {
            public IdTokenClaims(string consentIdClaimValue)
            {
                OpenbankingIntentId = new IndividualClaim(
                    true,
                    consentIdClaimValue,
                    null);
                Acr = new IndividualClaim(true, "urn:openbanking:psd2:ca", null);
                // Acr = new IndividualClaim(
                //     true,
                //     null,
                //     new[]
                //     {
                //         "urn:openbanking:psd2:sca",
                //         "urn:openbanking:psd2:ca"
                //     });
            }

            [JsonProperty("openbanking_intent_id")]
            public IndividualClaim OpenbankingIntentId { get; set; }

            [JsonProperty("acr")]
            public IndividualClaim Acr { get; set; }
        }
    }
}
