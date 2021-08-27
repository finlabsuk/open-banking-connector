// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

using System;

namespace FinnovationLabs.OpenBanking.Library.Connector.UkDcrApi.V3p1.Models
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class PutClientRegistrationOKResponse : OBRegistrationProperties1
    {
        /// <summary>
        /// Initializes a new instance of the PutClientRegistrationOKResponse
        /// class.
        /// </summary>
        public PutClientRegistrationOKResponse()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the PutClientRegistrationOKResponse
        /// class.
        /// </summary>
        /// <param name="tokenEndpointAuthMethod">Possible values include:
        /// 'private_key_jwt', 'tls_client_auth'</param>
        /// <param name="applicationType">Possible values include: 'web',
        /// 'mobile'</param>
        /// <param name="idTokenSignedResponseAlg">Possible values include:
        /// 'RS256', 'PS256', 'ES256'</param>
        /// <param name="requestObjectSigningAlg">Possible values include:
        /// 'RS256', 'PS256', 'ES256'</param>
        /// <param name="clientId">OAuth 2.0 client identifier string</param>
        /// <param name="clientSecret">OAuth 2.0 client secret string</param>
        /// <param name="clientIdIssuedAt">Time at which the client identifier
        /// was issued expressed as seconds since 1970-01-01T00:00:00Z as
        /// measured in UTC</param>
        /// <param name="clientSecretExpiresAt">Time at which the client secret
        /// will expire expressed as seconds since 1970-01-01T00:00:00Z as
        /// measured in UTC. Set to 0 if does not expire</param>
        /// <param name="tokenEndpointAuthSigningAlg">Possible values include:
        /// 'RS256', 'PS256', 'ES256'</param>
        public PutClientRegistrationOKResponse(
            IList<string> redirectUris,
            OBRegistrationProperties1tokenEndpointAuthMethodEnum tokenEndpointAuthMethod,
            IList<OBRegistrationProperties1grantTypesItemEnum> grantTypes,
            string softwareStatement,
            OBRegistrationProperties1applicationTypeEnum applicationType,
            SupportedAlgorithmsEnum idTokenSignedResponseAlg,
            SupportedAlgorithmsEnum requestObjectSigningAlg,
            string tlsClientAuthDn,
            string clientId = default(string),
            string clientSecret = default(string),
            DateTimeOffset? clientIdIssuedAt = default(DateTimeOffset?),
            DateTimeOffset? clientSecretExpiresAt = default(DateTimeOffset?),
            IList<OBRegistrationProperties1responseTypesItemEnum> responseTypes =
                default(IList<OBRegistrationProperties1responseTypesItemEnum>),
            string softwareId = default(string),
            IList<string> scope = default(IList<string>),
            SupportedAlgorithmsEnum? tokenEndpointAuthSigningAlg = default(SupportedAlgorithmsEnum?))
            : base(
                redirectUris,
                tokenEndpointAuthMethod,
                grantTypes,
                softwareStatement,
                applicationType,
                idTokenSignedResponseAlg,
                requestObjectSigningAlg,
                tlsClientAuthDn,
                clientId,
                clientSecret,
                clientIdIssuedAt,
                clientSecretExpiresAt,
                responseTypes,
                softwareId,
                scope,
                tokenEndpointAuthSigningAlg)
        {
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
        }
    }
}
