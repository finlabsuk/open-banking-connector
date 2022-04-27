// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response
{
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
        TokenEndpointAuthMethodEnum TokenEndpointAuthMethod { get; }

        /// <summary>
        ///     Custom behaviour, usually bank-specific, to handle quirks, formatting issues, etc.
        ///     For a well-behaved bank, normally this object should be null.
        /// </summary>
        CustomBehaviour? CustomBehaviour { get; }

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        string ExternalApiId { get; }
    }

    /// <summary>
    ///     Response to BankRegistration ReadLocal requests.
    /// </summary>
    public class BankRegistrationReadLocalResponse : BaseResponse, IBankRegistrationPublicQuery
    {
        public BankRegistrationReadLocalResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            Guid bankId,
            string softwareStatementProfileId,
            string? softwareStatementAndCertificateProfileOverrideCase,
            DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion,
            RegistrationScopeEnum registrationScope,
            string registrationEndpoint,
            string tokenEndpoint,
            string authorizationEndpoint,
            TokenEndpointAuthMethodEnum tokenEndpointAuthMethod,
            CustomBehaviour? customBehaviour,
            string externalApiId) : base(id, created, createdBy)
        {
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
            ExternalApiId = externalApiId;
        }

        public Guid BankId { get; }
        public string SoftwareStatementProfileId { get; }
        public string? SoftwareStatementAndCertificateProfileOverrideCase { get; }
        public DynamicClientRegistrationApiVersion DynamicClientRegistrationApiVersion { get; }
        public RegistrationScopeEnum RegistrationScope { get; }
        public string RegistrationEndpoint { get; }
        public string TokenEndpoint { get; }
        public string AuthorizationEndpoint { get; }
        public TokenEndpointAuthMethodEnum TokenEndpointAuthMethod { get; }
        public CustomBehaviour? CustomBehaviour { get; }
        public string ExternalApiId { get; }
    }


    /// <summary>
    ///     Response to BankRegistration Read and Create requests
    /// </summary>
    public class BankRegistrationReadResponse : BankRegistrationReadLocalResponse
    {
        public BankRegistrationReadResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            Guid bankId,
            string softwareStatementProfileId,
            string? softwareStatementAndCertificateProfileOverrideCase,
            DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion,
            RegistrationScopeEnum registrationScope,
            string registrationEndpoint,
            string tokenEndpoint,
            string authorizationEndpoint,
            TokenEndpointAuthMethodEnum tokenEndpointAuthMethod,
            CustomBehaviour? customBehaviour,
            string externalApiId,
            ClientRegistrationModelsPublic.OBClientRegistration1Response? externalApiResponse) : base(
            id,
            created,
            createdBy,
            bankId,
            softwareStatementProfileId,
            softwareStatementAndCertificateProfileOverrideCase,
            dynamicClientRegistrationApiVersion,
            registrationScope,
            registrationEndpoint,
            tokenEndpoint,
            authorizationEndpoint,
            tokenEndpointAuthMethod,
            customBehaviour,
            externalApiId)
        {
            ExternalApiResponse = externalApiResponse;
        }

        public ClientRegistrationModelsPublic.OBClientRegistration1Response? ExternalApiResponse { get; }
    }
}
