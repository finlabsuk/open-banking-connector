// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Obie;

/// Payload of Open Banking Software Statement Assertion. Fields can be added as required
public class SsaPayload
{
    private static readonly Dictionary<string, RegistrationScopeEnum> SoftwareRoleToApiType =
        new()
        {
            ["AISP"] = RegistrationScopeEnum.AccountAndTransaction,
            ["PISP"] = RegistrationScopeEnum.PaymentInitiation,
            ["CBPII"] = RegistrationScopeEnum.FundsConfirmation
        };

    [JsonProperty("software_on_behalf_of_org")]
    public string SoftwareOnBehalfOfOrg = null!;

    [JsonProperty("software_id")]
    public string SoftwareId { get; set; } = null!;

    [JsonProperty("software_client_id")]
    public string SoftwareClientId { get; set; } = null!;

    [JsonProperty("software_client_name")]
    public string SoftwareClientName { get; set; } = null!;

    [JsonProperty("software_client_description")]
    public string SoftwareClientDescription { get; set; } = null!;

    [JsonProperty("software_version")]
    public string SoftwareVersion { get; set; } = null!;

    [JsonProperty("software_client_uri")]
    public string SoftwareClientUri { get; set; } = null!;

    [JsonProperty("software_redirect_uris")]
    public List<string> SoftwareRedirectUris { get; set; } = null!;

    [JsonProperty("software_roles")]
    public string[] SoftwareRoles { get; set; } = null!;

    [JsonProperty("org_id")]
    public string OrgId { get; set; } = null!;

    [JsonProperty("org_name")]
    public string OrgName { get; set; } = null!;

    public RegistrationScopeEnum RegistrationScope =>
        SoftwareRoles.Select(role => SoftwareRoleToApiType[role]).Aggregate(
            RegistrationScopeEnum.None,
            (current, next) => current | next);
}
