// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration
{
    /// <summary>
    ///     Custom behaviour, usually bank-specific, to handle quirks, formatting issues, etc.
    ///     For a well-behaved bank, this object should be full of nulls (empty).
    /// </summary>
    public class CustomBehaviour
    {
        public OpenIdConfigurationOverrides? OpenIdConfigurationOverrides { get; set; }
        public BankRegistrationClaimsOverrides? BankRegistrationClaimsOverrides { get; set; }
        public BankRegistrationClaimsJsonOptions? BankRegistrationClaimsJsonOptions { get; set; }
        public BankRegistrationResponseJsonOptions? BankRegistrationResponseJsonOptions { get; set; }
        public BankRegistrationResponseOverrides? BankRegistrationResponseOverrides { get; set; }
        public OAuth2RequestObjectClaimsOverrides? OAuth2RequestObjectClaimsOverrides { get; set; }

        /// <summary>
        ///     Set Content-Type header to "application/jose" rather than "application/jwt" when POSTing to bank
        ///     registration endpoint
        /// </summary>
        public bool? UseApplicationJoseNotApplicationJwtContentTypeHeader { get; set; }

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
        public bool? UseTransportCertificateDnWithStringNotHexDottedDecimalAttributeValues { get; set; }
    }
}
