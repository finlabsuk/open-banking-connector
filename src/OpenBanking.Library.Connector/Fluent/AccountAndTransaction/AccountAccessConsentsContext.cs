// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;
using AccountAccessConsentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request.AccountAccessConsent;
using AccountAccessConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request.
    AccountAccessConsentAuthContext;
using AccountAccessConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.
    AccountAccessConsentAuthContext;
using BankApiSetPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankApiSet;
using BankRegistrationPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankRegistration;
using AccountAccessConsentAuthContext =
    FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction.AccountAccessConsentAuthContextPost;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.AccountAndTransaction
{
    public interface IAccountAccessConsentsContext :
        IEntityContext<AccountAccessConsentRequest,
            IAccountAccessConsentPublicQuery,
            AccountAccessConsentReadResponse, AccountAccessConsentReadLocalResponse>
    {
        /// <summary>
        ///     API for AuthorisationRedirectObject which corresponds to data received from bank following user
        ///     authorisation of consent.
        /// </summary>
        ILocalEntityContext<AccountAccessConsentAuthContextRequest,
                IAccountAccessConsentAuthContextPublicQuery,
                AccountAccessConsentAuthContextCreateLocalResponse,
                AccountAccessConsentAuthContextReadLocalResponse>
            AuthContexts { get; }
    }

    internal interface IAccountAccessConsentsContextInternal :
        IAccountAccessConsentsContext,
        IEntityContextInternal<AccountAccessConsentRequest,
            IAccountAccessConsentPublicQuery,
            AccountAccessConsentReadResponse, AccountAccessConsentReadLocalResponse> { }

    internal class AccountAccessConsentsConsentContext :
        ObjectContextBase<AccountAccessConsent>,
        IAccountAccessConsentsContextInternal
    {
        private readonly ISharedContext _sharedContext;

        public AccountAccessConsentsConsentContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _sharedContext = sharedContext;
            PostObject = new AccountAccessConsentPost(
                sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsent>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation,
                sharedContext.ApiVariantMapper,
                sharedContext.DbService.GetDbEntityMethodsClass<BankApiSetPersisted>(),
                sharedContext.DbService.GetDbEntityMethodsClass<BankRegistrationPersisted>());
            ReadObject = new AccountAccessConsentGet(
                sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsent>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation,
                sharedContext.ApiVariantMapper);
            ReadLocalObject =
                new LocalEntityGet<AccountAccessConsent, IAccountAccessConsentPublicQuery,
                    AccountAccessConsentReadLocalResponse>(
                    sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsent>(),
                    sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    sharedContext.TimeProvider,
                    sharedContext.SoftwareStatementProfileCachedRepo,
                    sharedContext.Instrumentation);
        }

        public ILocalEntityContext<AccountAccessConsentAuthContextRequest,
            IAccountAccessConsentAuthContextPublicQuery,
            AccountAccessConsentAuthContextCreateLocalResponse,
            AccountAccessConsentAuthContextReadLocalResponse> AuthContexts =>
            new LocalEntityContextInternal<AccountAccessConsentAuthContextPersisted,
                AccountAccessConsentAuthContextRequest,
                IAccountAccessConsentAuthContextPublicQuery,
                AccountAccessConsentAuthContextCreateLocalResponse,
                AccountAccessConsentAuthContextReadLocalResponse>(
                _sharedContext,
                new AccountAccessConsentAuthContext(
                    _sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsentAuthContextPersisted>(),
                    _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    _sharedContext.TimeProvider,
                    _sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsent>(),
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.Instrumentation));

        public IObjectRead<AccountAccessConsentReadResponse> ReadObject { get; }

        public IObjectPost<AccountAccessConsentRequest, AccountAccessConsentReadResponse> PostObject { get; }

        public IObjectReadLocal<IAccountAccessConsentPublicQuery, AccountAccessConsentReadLocalResponse> ReadLocalObject
        {
            get;
        }
    }
}
