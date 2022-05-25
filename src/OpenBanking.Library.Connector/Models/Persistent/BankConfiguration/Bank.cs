// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration
{
    /// <summary>
    ///     Persisted type for Bank.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class Bank :
        BaseEntity,
        IBankPublicQuery
    {
        public Bank(
            Guid id,
            string? reference,
            bool isDeleted,
            DateTimeOffset isDeletedModified,
            string? isDeletedModifiedBy,
            DateTimeOffset created,
            string? createdBy,
            string? issuerUrl,
            string financialId,
            string registrationEndpoint,
            string tokenEndpoint,
            string authorizationEndpoint,
            string jwksUri,
            OAuth2ResponseMode defaultResponseMode,
            DynamicClientRegistrationApiVersion dcrApiVersion,
            CustomBehaviourClass? customBehaviour,
            bool supportsSca) : base(
            id,
            reference,
            isDeleted,
            isDeletedModified,
            isDeletedModifiedBy,
            created,
            createdBy)
        {
            IssuerUrl = issuerUrl;
            FinancialId = financialId;
            RegistrationEndpoint = registrationEndpoint;
            TokenEndpoint = tokenEndpoint;
            AuthorizationEndpoint = authorizationEndpoint;
            JwksUri = jwksUri;
            DefaultResponseMode = defaultResponseMode;
            DcrApiVersion = dcrApiVersion;
            CustomBehaviour = customBehaviour;
            SupportsSca = supportsSca;
        }

        public IList<BankRegistration> BankRegistrationsNavigation { get; } = new List<BankRegistration>();

        public IList<AccountAndTransactionApiEntity> AccountAndTransactionApisNavigation { get; } =
            new List<AccountAndTransactionApiEntity>();

        public IList<PaymentInitiationApiEntity> PaymentInitiationApisNavigation { get; } =
            new List<PaymentInitiationApiEntity>();

        public IList<VariableRecurringPaymentsApiEntity> VariableRecurringPaymentsApisNavigation { get; } =
            new List<VariableRecurringPaymentsApiEntity>();

        /// <summary>
        ///     JWK Set URI (normally supplied from OpenID Configuration)
        /// </summary>
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

    internal partial class Bank :
        ISupportsFluentLocalEntityGet<BankResponse>
    {
        public BankResponse PublicGetLocalResponse => new(
            Id,
            Created,
            CreatedBy,
            Reference,
            JwksUri,
            DefaultResponseMode,
            SupportsSca,
            IssuerUrl,
            FinancialId,
            RegistrationEndpoint,
            TokenEndpoint,
            AuthorizationEndpoint,
            DcrApiVersion,
            CustomBehaviour);
    }
}
