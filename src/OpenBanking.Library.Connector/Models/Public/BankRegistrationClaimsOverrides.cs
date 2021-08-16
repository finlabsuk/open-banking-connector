// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Newtonsoft.Json;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.UkDcrApi.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public
{
    /// <summary>
    ///     Class used to specify overrides to default settings for bank client registration claims.
    ///     Default property values do not change anything so set properties to alter client
    ///     registration claims.
    /// </summary>
    public class BankRegistrationClaimsOverrides
    {
        public bool IssuerIsSoftwareStatementXFapiFinancialId { get; set; } = false;

        [JsonProperty("aud")]
        public string? Audience { get; set; }

        [JsonProperty("token_endpoint_auth_method")]
        public ClientRegistrationModelsPublic.OBRegistrationProperties1tokenEndpointAuthMethodEnum?
            TokenEndpointAuthMethod { get; set; }

        [JsonProperty("grant_types")]
        public IList<ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum>? GrantTypes
        {
            get;
            set;
        }

        // Notionally a nullable bool, but Json serialisation doesn't handle well. Assume false as default.
        // [JsonProperty("scope__useStringArray")]
        // public bool ScopeUseStringArray { get; set; }

        [JsonProperty("token_endpoint_auth_signing_alg")]
        public string? TokenEndpointAuthSigningAlgorithm { get; set; }

        [JsonProperty(PropertyName = "subject_type")]
        public string? SubjectType { get; set; }
    }
}
