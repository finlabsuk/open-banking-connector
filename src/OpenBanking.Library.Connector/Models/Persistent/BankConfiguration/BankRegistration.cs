// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using OAuth2RequestObjectClaimsOverridesRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.OAuth2RequestObjectClaimsOverrides;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration
{
    internal class ExternalApiObject : IBankRegistrationExternalApiObjectPublicQuery
    {
        public ExternalApiObject(string externalApiId, string? externalApiSecret, string? registrationAccessToken)
        {
            ExternalApiId = externalApiId;
            ExternalApiSecret = externalApiSecret;
            RegistrationAccessToken = registrationAccessToken;
        }

        public string? ExternalApiSecret { get; }

        public string? RegistrationAccessToken { get; }

        public string ExternalApiId { get; }
    }

    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class BankRegistration :
        BaseEntity,
        IBankRegistrationPublicQuery
    {
        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        [Column("external_api_id")]
        private readonly string _externalApiId;

        /// <summary>
        ///     External API secret. Present to allow use of legacy token auth method "client_secret_basic" in sandboxes etc.
        /// </summary>
        [Column("external_api_secret")]
        private readonly string? _externalApiSecret;

        /// <summary>
        ///     External API registration access token. Sometimes used to support registration adjustments etc.
        /// </summary>
        [Column("registration_access_token")]
        private readonly string? _registrationAccessToken;

        public BankRegistration(
            Guid id,
            string? reference,
            bool isDeleted,
            DateTimeOffset isDeletedModified,
            string? isDeletedModifiedBy,
            DateTimeOffset created,
            string? createdBy,
            Guid bankId,
            string softwareStatementProfileId,
            string? softwareStatementAndCertificateProfileOverrideCase,
            RegistrationScopeEnum registrationScope,
            DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion,
            string tokenEndpoint,
            string authorizationEndpoint,
            string registrationEndpoint,
            TokenEndpointAuthMethod tokenEndpointAuthMethod,
            CustomBehaviour? customBehaviour,
            string externalApiId,
            string? externalApiSecret,
            string? registrationAccessToken) : base(
            id,
            reference,
            isDeleted,
            isDeletedModified,
            isDeletedModifiedBy,
            created,
            createdBy)
        {
            BankId = bankId;
            SoftwareStatementProfileId = softwareStatementProfileId;
            SoftwareStatementAndCertificateProfileOverrideCase = softwareStatementAndCertificateProfileOverrideCase;
            RegistrationScope = registrationScope;
            DynamicClientRegistrationApiVersion = dynamicClientRegistrationApiVersion;
            TokenEndpoint = tokenEndpoint;
            AuthorizationEndpoint = authorizationEndpoint;
            RegistrationEndpoint = registrationEndpoint;
            TokenEndpointAuthMethod = tokenEndpointAuthMethod;
            CustomBehaviour = customBehaviour;
            _externalApiId = externalApiId;
            _externalApiSecret = externalApiSecret;
            _registrationAccessToken = registrationAccessToken;
        }

        [ForeignKey("BankId")]
        public Bank BankNavigation { get; set; } = null!;

        public ExternalApiObject ExternalApiObject => new(
            _externalApiId,
            _externalApiSecret,
            _registrationAccessToken);

        /// <summary>
        ///     ID of SoftwareStatementProfile to use in association with BankRegistration
        /// </summary>
        public string SoftwareStatementProfileId { get; }

        public string? SoftwareStatementAndCertificateProfileOverrideCase { get; }

        /// <summary>
        ///     API version used for DCR requests (POST, GET etc)
        /// </summary>
        public DynamicClientRegistrationApiVersion DynamicClientRegistrationApiVersion { get; }

        /// <summary>
        ///     Functional APIs used for bank registration.
        /// </summary>
        public RegistrationScopeEnum RegistrationScope { get; }

        /// <summary>
        ///     Registration endpoint (normally supplied from OpenID Configuration)
        /// </summary>
        public string RegistrationEndpoint { get; }

        /// <summary>
        ///     Token endpoint (normally supplied from OpenID Configuration)
        /// </summary>
        public string TokenEndpoint { get; }

        /// <summary>
        ///     Authorization endpoint (normally supplied from OpenID Configuration)
        /// </summary>
        public string AuthorizationEndpoint { get; }

        /// <summary>
        ///     Token endpoint authorisation method
        /// </summary>
        public TokenEndpointAuthMethod TokenEndpointAuthMethod { get; }

        /// <summary>
        ///     Custom behaviour, usually bank-specific, to handle quirks, formatting issues, etc.
        ///     For a well-behaved bank, normally this object should be null.
        /// </summary>
        public CustomBehaviour? CustomBehaviour { get; }

        /// <summary>
        ///     Bank with which this BankRegistration is associated.
        /// </summary>
        public Guid BankId { get; }

        IBankRegistrationExternalApiObjectPublicQuery IBankRegistrationPublicQuery.ExternalApiObject =>
            ExternalApiObject;
    }

    internal partial class BankRegistration :
        ISupportsFluentLocalEntityGet<BankRegistrationReadLocalResponse>
    {
        public BankRegistrationReadLocalResponse PublicGetLocalResponse => new(
            Id,
            Created,
            CreatedBy,
            Reference,
            new ExternalApiObjectResponse(_externalApiId),
            BankId,
            SoftwareStatementProfileId,
            SoftwareStatementAndCertificateProfileOverrideCase,
            DynamicClientRegistrationApiVersion,
            RegistrationScope,
            RegistrationEndpoint,
            TokenEndpoint,
            AuthorizationEndpoint,
            TokenEndpointAuthMethod,
            CustomBehaviour);
    }
}
