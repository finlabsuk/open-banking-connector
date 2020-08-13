// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// Struct corresponding to payload of Open Banking Software Statement type. Fields can be added as required
    public class SoftwareStatementPayload
    {
        private static readonly Dictionary<string, string> SoftwareRoleToScope = new Dictionary<string, string>
        {
            ["AISP"] = "accounts",
            ["PISP"] = "payments",
            ["CBPII"] = "fundsconfirmations"
        };

        [JsonProperty("software_on_behalf_of_org")]
        public string SoftwareOnBehalfOfOrg;

        [JsonProperty("software_id")]
        public string SoftwareId { get; set; }

        [JsonProperty("software_client_id")]
        public string SoftwareClientId { get; set; }

        [JsonProperty("software_client_name")]
        public string SoftwareClientName { get; set; }

        [JsonProperty("software_client_description")]
        public string SoftwareClientDescription { get; set; }

        [JsonProperty("software_version")]
        public float SoftwareVersion { get; set; }

        [JsonProperty("software_client_uri")]
        public string SoftwareClientUri { get; set; }

        [JsonProperty("software_redirect_uris")]
        public string[] SoftwareRedirectUris { get; set; }

        [JsonProperty("software_roles")]
        public string[] SoftwareRoles { get; set; }

        [JsonProperty("org_id")]
        public string OrgId { get; set; }

        [JsonProperty("org_name")]
        public string OrgName { get; set; }

        /// Scope list computed from software_roles
        public string Scope
        {
            get
            {
                // insert "openid" if not present
                IEnumerable<string> mutatingSoftwareRoles = SoftwareRoles.Select(role => SoftwareRoleToScope[role])
                    .Prepend("openid")
                    .Distinct();

                return mutatingSoftwareRoles.JoinString(" ");
            }
        }
    }
}
