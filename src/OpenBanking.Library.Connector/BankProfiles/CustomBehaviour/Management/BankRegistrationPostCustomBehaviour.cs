// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.Management;

/// <summary>
///     Class used to specify options for BankRegistration endpoints at
///     bank API.
///     Default (e.g. null) property values do not lead to changes.
/// </summary>
public class BankRegistrationPostCustomBehaviour
{
    // JsonConverter to use for ClientIdIssuedAt claim response
    public DateTimeOffsetUnixConverterEnum? ClientIdIssuedAtClaimResponseJsonConverter { get; set; }

    // JsonConverter to use for scope claim
    public DelimitedStringConverterOptions? ScopeClaimJsonConverter { get; set; }

    // JsonConverter to use for scope claim response
    public DelimitedStringConverterOptions? ScopeClaimResponseJsonConverter { get; set; }

    // Override aud claim
    public string? AudClaim { get; set; }

    // Overwrite subject_type claim
    public string? SubjectTypeClaim { get; set; }

    // Override grant_types claim
    public IList<ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum>? GrantTypesClaim
    {
        get;
        set;
    }

    // Override grant_types claim response (used to correct error in response)
    public IList<ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum>?
        GrantTypesClaimResponse { get; set; }

    /// <summary>
    ///     Set Content-Type header to "application/jose" rather than "application/jwt" when POSTing to bank
    ///     registration endpoint
    /// </summary>
    public bool? UseApplicationJoseNotApplicationJwtContentTypeHeader { get; set; }

    /// <summary>
    ///     For registration, transport certificate subject DN must be specified. OID 2.5.4.97 (organizationIdentifier) may use
    ///     a
    ///     dotted-decimal AttributeType.
    ///     Note that https://datatracker.ietf.org/doc/html/rfc4514#section-2.4 specifies hex encoding for an AttributeValue
    ///     corresponding to a dotted-decimal
    ///     AttributeType.
    /// </summary>
    public SubjectDnOrgIdEncoding? TransportCertificateSubjectDnOrgIdEncoding { get; set; }
}
