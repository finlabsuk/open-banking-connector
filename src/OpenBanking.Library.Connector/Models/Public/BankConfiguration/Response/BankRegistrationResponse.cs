﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        ///     ID of SoftwareStatementProfile to use in association with BankRegistration
        /// </summary>
        string SoftwareStatementProfileId { get; }

        string? SoftwareStatementProfileOverride { get; }

        /// <summary>
        ///     Token endpoint authorisation method
        /// </summary>
        TokenEndpointAuthMethod TokenEndpointAuthMethod { get; }


        /// <summary>
        ///     Functional APIs used for bank registration.
        /// </summary>
        RegistrationScopeEnum RegistrationScope { get; }


        /// <summary>
        ///     Bank with which this BankRegistration is associated.
        /// </summary>
        Guid BankId { get; }


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
            string softwareStatementProfileId,
            string? softwareStatementAndCertificateProfileOverrideCase,
            TokenEndpointAuthMethod tokenEndpointAuthMethod,
            RegistrationScopeEnum registrationScope,
            Guid bankId,
            ExternalApiObjectResponse externalApiObject) : base(id, created, createdBy, reference)
        {
            SoftwareStatementProfileId = softwareStatementProfileId;
            SoftwareStatementProfileOverride = softwareStatementAndCertificateProfileOverrideCase;
            TokenEndpointAuthMethod = tokenEndpointAuthMethod;
            RegistrationScope = registrationScope;
            BankId = bankId;
            ExternalApiObject = externalApiObject;
        }


        public ExternalApiObjectResponse ExternalApiObject { get; }

        public string SoftwareStatementProfileId { get; }

        public string? SoftwareStatementProfileOverride { get; }

        /// <summary>
        ///     Token endpoint authorisation method
        /// </summary>
        public TokenEndpointAuthMethod TokenEndpointAuthMethod { get; }


        /// <summary>
        ///     Functional APIs used for bank registration.
        /// </summary>
        public RegistrationScopeEnum RegistrationScope { get; }


        /// <summary>
        ///     Bank with which this BankRegistration is associated.
        /// </summary>
        public Guid BankId { get; }

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
            string softwareStatementProfileId,
            string? softwareStatementAndCertificateProfileOverrideCase,
            TokenEndpointAuthMethod tokenEndpointAuthMethod,
            RegistrationScopeEnum registrationScope,
            Guid bankId,
            ExternalApiObjectResponse externalApiObject,
            ClientRegistrationModelsPublic.OBClientRegistration1Response? externalApiResponse) : base(
            id,
            created,
            createdBy,
            reference,
            softwareStatementProfileId,
            softwareStatementAndCertificateProfileOverrideCase,
            tokenEndpointAuthMethod,
            registrationScope,
            bankId,
            externalApiObject)
        {
            ExternalApiResponse = externalApiResponse;
        }

        public ClientRegistrationModelsPublic.OBClientRegistration1Response? ExternalApiResponse { get; }
    }
}
