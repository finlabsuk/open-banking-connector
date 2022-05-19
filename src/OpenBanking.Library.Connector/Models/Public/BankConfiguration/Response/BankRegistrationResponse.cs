// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response
{
    public interface IBankRegistrationExternalApiObjectPublicQuery
    {
        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        string ExternalApiId { get; }
    }

    public class ExternalApiObjectResponse : IBankRegistrationExternalApiObjectPublicQuery
    {
        public ExternalApiObjectResponse(string externalApiId)
        {
            ExternalApiId = externalApiId;
        }

        public string ExternalApiId { get; }
    }

    public interface IBankRegistrationPublicQuery : IBaseQuery
    {
        /// <summary>
        ///     Bank with which this BankRegistration is associated.
        /// </summary>
        Guid BankId { get; }

        /// <summary>
        ///     ID of SoftwareStatementProfile to use in association with BankRegistration
        /// </summary>
        string SoftwareStatementProfileId { get; }

        string? SoftwareStatementAndCertificateProfileOverrideCase { get; }

        /// <summary>
        ///     API version used for DCR requests (POST, GET etc)
        /// </summary>
        DynamicClientRegistrationApiVersion DynamicClientRegistrationApiVersion { get; }

        /// <summary>
        ///     Functional APIs used for bank registration.
        /// </summary>
        RegistrationScopeEnum RegistrationScope { get; }

        /// <summary>
        ///     Registration endpoint (normally supplied from OpenID Configuration)
        /// </summary>
        string RegistrationEndpoint { get; }

        /// <summary>
        ///     Token endpoint (normally supplied from OpenID Configuration)
        /// </summary>
        string TokenEndpoint { get; }

        /// <summary>
        ///     Authorization endpoint (normally supplied from OpenID Configuration)
        /// </summary>
        string AuthorizationEndpoint { get; }

        /// <summary>
        ///     Token endpoint authorisation method
        /// </summary>
        TokenEndpointAuthMethod TokenEndpointAuthMethod { get; }

        /// <summary>
        ///     Custom behaviour, usually bank-specific, to handle quirks, formatting issues, etc.
        ///     For a well-behaved bank, normally this object should be null.
        /// </summary>
        CustomBehaviourClass? CustomBehaviour { get; }

        IBankRegistrationExternalApiObjectPublicQuery ExternalApiObject { get; }
    }

    /// <summary>
    ///     Response to BankRegistration ReadLocal requests.
    /// </summary>
    public class BankRegistrationReadLocalResponse : BaseResponse, IBankRegistrationPublicQuery
    {
        internal BankRegistrationReadLocalResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            ExternalApiObjectResponse externalApiObject,
            Guid bankId,
            string softwareStatementProfileId,
            string? softwareStatementAndCertificateProfileOverrideCase,
            DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion,
            RegistrationScopeEnum registrationScope,
            string registrationEndpoint,
            string tokenEndpoint,
            string authorizationEndpoint,
            TokenEndpointAuthMethod tokenEndpointAuthMethod,
            CustomBehaviourClass? customBehaviour) : base(id, created, createdBy, reference)
        {
            ExternalApiObject = externalApiObject;
            BankId = bankId;
            SoftwareStatementProfileId = softwareStatementProfileId;
            SoftwareStatementAndCertificateProfileOverrideCase = softwareStatementAndCertificateProfileOverrideCase;
            DynamicClientRegistrationApiVersion = dynamicClientRegistrationApiVersion;
            RegistrationScope = registrationScope;
            RegistrationEndpoint = registrationEndpoint;
            TokenEndpoint = tokenEndpoint;
            AuthorizationEndpoint = authorizationEndpoint;
            TokenEndpointAuthMethod = tokenEndpointAuthMethod;
            CustomBehaviour = customBehaviour;
        }

        public ExternalApiObjectResponse ExternalApiObject { get; }
        public Guid BankId { get; }
        public string SoftwareStatementProfileId { get; }
        public string? SoftwareStatementAndCertificateProfileOverrideCase { get; }
        public DynamicClientRegistrationApiVersion DynamicClientRegistrationApiVersion { get; }
        public RegistrationScopeEnum RegistrationScope { get; }
        public string RegistrationEndpoint { get; }
        public string TokenEndpoint { get; }
        public string AuthorizationEndpoint { get; }
        public TokenEndpointAuthMethod TokenEndpointAuthMethod { get; }
        public CustomBehaviourClass? CustomBehaviour { get; }

        IBankRegistrationExternalApiObjectPublicQuery IBankRegistrationPublicQuery.ExternalApiObject =>
            ExternalApiObject;
    }

    /// <summary>
    ///     Response to BankRegistration Read and Create requests
    /// </summary>
    public class BankRegistrationReadResponse : BankRegistrationReadLocalResponse
    {
        internal BankRegistrationReadResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            ExternalApiObjectResponse externalApiObject,
            Guid bankId,
            string softwareStatementProfileId,
            string? softwareStatementAndCertificateProfileOverrideCase,
            DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion,
            RegistrationScopeEnum registrationScope,
            string registrationEndpoint,
            string tokenEndpoint,
            string authorizationEndpoint,
            TokenEndpointAuthMethod tokenEndpointAuthMethod,
            CustomBehaviourClass? customBehaviour,
            ClientRegistrationModelsPublic.OBClientRegistration1Response? externalApiResponse) : base(
            id,
            created,
            createdBy,
            reference,
            externalApiObject,
            bankId,
            softwareStatementProfileId,
            softwareStatementAndCertificateProfileOverrideCase,
            dynamicClientRegistrationApiVersion,
            registrationScope,
            registrationEndpoint,
            tokenEndpoint,
            authorizationEndpoint,
            tokenEndpointAuthMethod,
            customBehaviour)
        {
            ExternalApiResponse = externalApiResponse;
        }

        public ClientRegistrationModelsPublic.OBClientRegistration1Response? ExternalApiResponse { get; }
    }
}
