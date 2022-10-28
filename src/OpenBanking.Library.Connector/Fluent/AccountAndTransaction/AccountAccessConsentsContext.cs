// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using AccountAccessConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request.
    AccountAccessConsentAuthContext;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;
using AccountAccessConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.
    AccountAccessConsentAuthContext;
using BankRegistrationPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;
using AccountAccessConsentAuthContext =
    FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction.AccountAccessConsentAuthContextPost;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.AccountAndTransaction
{
    public interface IAccountAccessConsentsContext :
        IConsentContext<AccountAccessConsentRequest,
            IAccountAccessConsentPublicQuery,
            AccountAccessConsentResponse, AccountAccessConsentResponse>,
        IDeleteConsentContext
    {
        /// <summary>
        ///     API for AuthorisationRedirectObject which corresponds to data received from bank following user
        ///     authorisation of consent.
        /// </summary>
        ILocalEntityContext<AccountAccessConsentAuthContextRequest,
                IAccountAccessConsentAuthContextPublicQuery,
                AccountAccessConsentAuthContextCreateResponse,
                AccountAccessConsentAuthContextReadResponse>
            AuthContexts { get; }
    }

    internal interface IAccountAccessConsentsContextInternal :
        IAccountAccessConsentsContext,
        IConsentContextInternal<AccountAccessConsentRequest,
            IAccountAccessConsentPublicQuery,
            AccountAccessConsentResponse, AccountAccessConsentResponse>, IDeleteConsentContextInternal { }

    internal class AccountAccessConsentsConsentContext :
        IAccountAccessConsentsContextInternal
    {
        private readonly ISharedContext _sharedContext;

        public AccountAccessConsentsConsentContext(ISharedContext sharedContext)
        {
            _sharedContext = sharedContext;
            var accountAccessConsentOperations = new AccountAccessConsentOperations(
                sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsentPersisted>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation,
                sharedContext.ApiVariantMapper,
                new GrantPost(_sharedContext.ApiClient),
                sharedContext.DbService.GetDbEntityMethodsClass<AccountAndTransactionApiEntity>(),
                sharedContext.BankProfileDefinitions,
                sharedContext.DbService.GetDbEntityMethodsClass<BankRegistrationPersisted>());
            CreateObject = accountAccessConsentOperations;
            ReadObject = accountAccessConsentOperations;
            ReadLocalObject =
                new LocalEntityRead<AccountAccessConsentPersisted, IAccountAccessConsentPublicQuery,
                    AccountAccessConsentResponse>(
                    sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsentPersisted>(),
                    sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    sharedContext.TimeProvider,
                    sharedContext.SoftwareStatementProfileCachedRepo,
                    sharedContext.Instrumentation);
            DeleteObject =
                new AccountAccessConsentDelete(
                    sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsentPersisted>(),
                    sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    sharedContext.TimeProvider,
                    sharedContext.SoftwareStatementProfileCachedRepo,
                    sharedContext.Instrumentation,
                    new GrantPost(_sharedContext.ApiClient));
        }

        public ILocalEntityContext<AccountAccessConsentAuthContextRequest,
            IAccountAccessConsentAuthContextPublicQuery,
            AccountAccessConsentAuthContextCreateResponse,
            AccountAccessConsentAuthContextReadResponse> AuthContexts =>
            new LocalEntityContextInternal<AccountAccessConsentAuthContextPersisted,
                AccountAccessConsentAuthContextRequest,
                IAccountAccessConsentAuthContextPublicQuery,
                AccountAccessConsentAuthContextCreateResponse,
                AccountAccessConsentAuthContextReadResponse>(
                _sharedContext,
                new AccountAccessConsentAuthContext(
                    _sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsentAuthContextPersisted>(),
                    _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    _sharedContext.TimeProvider,
                    _sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsentPersisted>(),
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.Instrumentation));

        public IObjectRead<AccountAccessConsentResponse, ConsentReadParams> ReadObject { get; }

        public IObjectCreate<AccountAccessConsentRequest, AccountAccessConsentResponse, ConsentCreateParams>
            CreateObject { get; }

        public IObjectReadWithSearch<IAccountAccessConsentPublicQuery, AccountAccessConsentResponse, LocalReadParams>
            ReadLocalObject { get; }

        public IObjectDelete<ConsentDeleteParams> DeleteObject { get; }
    }
}
