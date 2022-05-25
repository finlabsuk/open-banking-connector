﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response
{
    public interface IBankPublicQuery : IBaseQuery
    {
        public string JwksUri { get; }

        public OAuth2ResponseMode DefaultResponseMode { get; }

        public bool SupportsSca { get; }

        public string? IssuerUrl { get; }
        public string FinancialId { get; }

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
        ///     API version used for DCR requests (POST, GET etc)
        /// </summary>
        public DynamicClientRegistrationApiVersion DcrApiVersion { get; }

        /// <summary>
        ///     Custom behaviour, usually bank-specific, to handle quirks, formatting issues, etc.
        ///     For a well-behaved bank, normally this object should be null.
        /// </summary>
        public CustomBehaviourClass? CustomBehaviour { get; }
    }

    /// <summary>
    ///     Response to GetLocal
    /// </summary>
    public class BankResponse : BaseResponse, IBankPublicQuery
    {
        public BankResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            string jwksUri,
            OAuth2ResponseMode defaultResponseMode,
            bool supportsSca,
            string? issuerUrl,
            string financialId,
            string registrationEndpoint,
            string tokenEndpoint,
            string authorizationEndpoint,
            DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion,
            CustomBehaviourClass? customBehaviour) : base(id, created, createdBy, reference)
        {
            JwksUri = jwksUri;
            DefaultResponseMode = defaultResponseMode;
            SupportsSca = supportsSca;
            IssuerUrl = issuerUrl;
            FinancialId = financialId;
            RegistrationEndpoint = registrationEndpoint;
            TokenEndpoint = tokenEndpoint;
            AuthorizationEndpoint = authorizationEndpoint;
            DcrApiVersion = dynamicClientRegistrationApiVersion;
            CustomBehaviour = customBehaviour;
        }

        public string JwksUri { get; }

        public OAuth2ResponseMode DefaultResponseMode { get; }

        public bool SupportsSca { get; }

        public string? IssuerUrl { get; }
        public string FinancialId { get; }

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
        ///     API version used for DCR requests (POST, GET etc)
        /// </summary>
        public DynamicClientRegistrationApiVersion DcrApiVersion { get; }

        /// <summary>
        ///     Custom behaviour, usually bank-specific, to handle quirks, formatting issues, etc.
        ///     For a well-behaved bank, normally this object should be null.
        /// </summary>
        public CustomBehaviourClass? CustomBehaviour { get; }
    }
}
