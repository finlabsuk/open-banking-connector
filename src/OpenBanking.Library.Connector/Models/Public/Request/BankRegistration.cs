// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.ClientRegistration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;
using FluentValidation.Results;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    public class BankRegistration : Base, ISupportsValidation
    {
        /// <summary>
        ///     Bank this registration is with.
        /// </summary>
        public Guid BankId { get; set; }

        /// <summary>
        ///     ID of software statement profile used to create bank registration. Only
        ///     IDs which have been specified at OBC startup via
        ///     <see cref="OpenBankingConnectorSettings.SoftwareStatementProfileIds" />
        ///     will be accepted.
        /// </summary>
        public string SoftwareStatementProfileId { get; set; } = null!;

        /// <summary>
        ///     Optional override case to use with software statement and certificate profiles. Override cases
        ///     can be used for bank-specific customisations to profiles, e.g. different transport certificate DN string.
        ///     When null no override case is specified.
        /// </summary>
        public string? SoftwareStatementAndCertificateProfileOverrideCase { get; set; } = null;

        /// <summary>
        ///     Functional APIs used for bank registration.
        ///     If not supplied, registration scope implied by software statement profile will be used.
        /// </summary>
        public RegistrationScope? RegistrationScope { get; set; }

        /// <summary>
        ///     Version of Open Banking Dynamic Client Registration API to use
        ///     for bank registration.
        /// </summary>
        public ClientRegistrationApiVersion ClientRegistrationApi { get; set; }

        /// <summary>
        ///     JSON string with Open ID configuration (well-known endpoint) response to be used
        ///     instead of calling bank API. When null, bank API endpoint provides response.
        /// </summary>
        public string? OpenIdConfigurationReplacement { get; set; } = null;

        /// <summary>
        ///     Set Content-Type header to "application/jose" rather than "application/jwt" when POSTing to bank
        ///     registration endpoint
        /// </summary>
        public bool UseApplicationJoseNotApplicationJwtContentTypeHeader { get; set; } = false;

        /// <summary>
        ///     Use
        ///     <see cref="TransportCertificateProfile.CertificateDnWithStringDottedDecimalAttributeValues" />
        ///     rather than
        ///     <see cref="TransportCertificateProfile.CertificateDnWithHexDottedDecimalAttributeValues" />
        ///     as transport certificate DN for registration. This setting is irrelevant where the
        ///     software statement profile specifies a <see cref="TransportCertificateType.OBLegacy" /> transport certificate.
        ///     https://datatracker.ietf.org/doc/html/rfc4514#section-2.4 specifies hex-encoding for a decimal-dotted
        ///     AttributeValue and thus this setting defaults to false.
        /// </summary>
        public bool UseTransportCertificateDnWithStringNotHexDottedDecimalAttributeValues { get; set; } = false;

        public OpenIdConfigurationOverrides? OpenIdConfigurationOverrides { get; set; }

        public BankRegistrationClaimsOverrides? BankRegistrationClaimsOverrides { get; set; }

        public BankRegistrationResponseJsonOptions? BankRegistrationResponseJsonOptions { get; set; }

        public BankRegistrationResponseOverrides? BankRegistrationResponseOverrides { get; set; }

        public OAuth2RequestObjectClaimsOverrides? OAuth2RequestObjectClaimsOverrides { get; set; }

        /// <summary>
        ///     If registration already exists for bank, allow creation of additional one. NB this may
        ///     disrupt existing registration depending on bank support for multiple registrations.
        /// </summary>
        public bool AllowMultipleRegistrations { get; set; } = false;

        public async Task<ValidationResult> ValidateAsync() =>
            await new BankRegistrationValidator()
                .ValidateAsync(this)!;
    }
}
