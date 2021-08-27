// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

using System;
using FinnovationLabs.OpenBanking.Library.Connector.ExternalApiBase;
using FinnovationLabs.OpenBanking.Library.Connector.ExternalApiBase.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.UkDcrApi.V3p3.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    [TargetApiEquivalent(typeof(V3p1.Models.OBClientRegistration1),
        ValueMappingSourceMembers = new string[]
        {
            "TlsClientAuthSubjectDn",
        },
        ValueMappingDestinationMembers = new []
        {
            "TlsClientAuthDn"
        },
        ValueMappings = new []
        {
            ValueMapping.StringIdentityValueConverter
        })
    ]
    [TargetApiEquivalent(typeof(V3p2.Models.OBClientRegistration1))]
    public partial class OBClientRegistration1 : OBRegistrationProperties1
    {
        /// <summary>
        /// Initializes a new instance of the OBClientRegistration1 class.
        /// </summary>
        public OBClientRegistration1()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the OBClientRegistration1 class.
        /// </summary>
        /// <param name="tokenEndpointAuthMethod">Possible values include:
        /// 'private_key_jwt', 'client_secret_jwt', 'client_secret_basic',
        /// 'client_secret_post', 'tls_client_auth'</param>
        /// <param name="applicationType">Possible values include: 'web',
        /// 'mobile'</param>
        /// <param name="idTokenSignedResponseAlg">Possible values include:
        /// 'RS256', 'PS256', 'ES256'</param>
        /// <param name="requestObjectSigningAlg">Possible values include:
        /// 'RS256', 'PS256', 'ES256'</param>
        /// <param name="iss">Unique identifier for the TPP. Implemented as
        /// Base62 encoded GUID</param>
        /// <param name="iat">The time at which the request was issued by the
        /// TPP  expressed as seconds since 1970-01-01T00:00:00Z as measured in
        /// UTC</param>
        /// <param name="exp">The time at which the request expires expressed
        /// as seconds since 1970-01-01T00:00:00Z as measured in UTC</param>
        /// <param name="aud">The audience for the request. This should be the
        /// unique identifier
        /// for the ASPSP issued by the issuer of the software statement.
        /// Implemented as Base62 encoded GUID
        /// </param>
        /// <param name="jti">Unique identifier for the JWT implemented as UUID
        /// v4</param>
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
        /// <param name="backchannelTokenDeliveryMode">As defined in CIBA -
        /// Registration and Discovery Metadata.  This value MUST be specified
        /// iff the grant_types includes  urn:openid:params:grant-type:ciba.
        /// Possible values include: 'poll', 'ping', 'push'</param>
        /// <param name="backchannelClientNotificationEndpoint">As defined in
        /// CIBA - Registration and Discovery Metadata.  This value MUST be
        /// specified iff the grant_types includes
        /// urn:openid:params:grant-type:ciba and
        /// backchannel_token_delivery_mode is not poll.  This must be a valid
        /// HTTPS URL</param>
        /// <param name="backchannelAuthenticationRequestSigningAlg">As defined
        /// in CIBA - Registration and Discovery Metadata.  This value MUST be
        /// specified iff the grant_types includes
        /// urn:openid:params:grant-type:ciba. Possible values include:
        /// 'RS256', 'PS256', 'ES256'</param>
        /// <param name="backchannelUserCodeParameterSupported">As defined in
        /// CIBA - Registration and Discovery Metadata.  This value MUST be
        /// specified only if the grant_types includes
        /// urn:openid:params:grant-type:ciba.  If specified, it MUST be set to
        /// false.</param>
        public OBClientRegistration1(IList<string> redirectUris, OBRegistrationProperties1tokenEndpointAuthMethodEnum tokenEndpointAuthMethod, IList<OBRegistrationProperties1grantTypesItemEnum> grantTypes, string scope, string softwareStatement, OBRegistrationProperties1applicationTypeEnum applicationType, SupportedAlgorithmsEnum idTokenSignedResponseAlg, SupportedAlgorithmsEnum requestObjectSigningAlg, string iss, DateTimeOffset iat, DateTimeOffset exp, string aud, string jti, string clientId = default(string), string clientSecret = default(string), DateTimeOffset? clientIdIssuedAt = default(DateTimeOffset?), DateTimeOffset? clientSecretExpiresAt = default(DateTimeOffset?), IList<OBRegistrationProperties1responseTypesItemEnum> responseTypes = default(IList<OBRegistrationProperties1responseTypesItemEnum>), string softwareId = default(string), SupportedAlgorithmsEnum? tokenEndpointAuthSigningAlg = default(SupportedAlgorithmsEnum?), string tlsClientAuthSubjectDn = default(string), OBRegistrationProperties1backchannelTokenDeliveryModeEnum? backchannelTokenDeliveryMode = default(OBRegistrationProperties1backchannelTokenDeliveryModeEnum?), string backchannelClientNotificationEndpoint = default(string), OBRegistrationProperties1backchannelAuthenticationRequestSigningAlgEnum? backchannelAuthenticationRequestSigningAlg = default(OBRegistrationProperties1backchannelAuthenticationRequestSigningAlgEnum?), bool? backchannelUserCodeParameterSupported = default(bool?))
            : base(redirectUris, tokenEndpointAuthMethod, grantTypes, scope, softwareStatement, applicationType, idTokenSignedResponseAlg, requestObjectSigningAlg, clientId, clientSecret, clientIdIssuedAt, clientSecretExpiresAt, responseTypes, softwareId, tokenEndpointAuthSigningAlg, tlsClientAuthSubjectDn, backchannelTokenDeliveryMode, backchannelClientNotificationEndpoint, backchannelAuthenticationRequestSigningAlg, backchannelUserCodeParameterSupported)
        {
            Iss = iss;
            Iat = iat;
            Exp = exp;
            Aud = aud;
            Jti = jti;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets unique identifier for the TPP. Implemented as Base62
        /// encoded GUID
        /// </summary>
        [JsonProperty(PropertyName = "iss")]
        public string Iss { get; set; }

        /// <summary>
        /// Gets or sets the time at which the request was issued by the TPP
        /// expressed as seconds since 1970-01-01T00:00:00Z as measured in UTC
        /// </summary>
        [JsonProperty(PropertyName = "iat")]
        [JsonConverter(typeof(DateTimeOffsetUnixConverter))]
        public DateTimeOffset Iat { get; set; }
        
        /// <summary>
        /// Gets or sets the time at which the request expires expressed as
        /// seconds since 1970-01-01T00:00:00Z as measured in UTC
        /// </summary>
        [JsonProperty(PropertyName = "exp")]
        [JsonConverter(typeof(DateTimeOffsetUnixConverter))]
        public DateTimeOffset Exp { get; set; }


        /// <summary>
        /// Gets or sets the audience for the request. This should be the
        /// unique identifier
        /// for the ASPSP issued by the issuer of the software statement.
        /// Implemented as Base62 encoded GUID
        ///
        /// </summary>
        [JsonProperty(PropertyName = "aud")]
        public string Aud { get; set; }

        /// <summary>
        /// Gets or sets unique identifier for the JWT implemented as UUID v4
        /// </summary>
        [JsonProperty(PropertyName = "jti")]
        public string Jti { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (Iss == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Iss");
            }
            if (Aud == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Aud");
            }
            if (Jti == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Jti");
            }
        }
    }
}
