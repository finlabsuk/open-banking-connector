// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
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
using AccountAccessConsentAuthContext =
    FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction.AccountAccessConsentAuthContextPost;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.AccountAndTransaction;

public interface IAccountAccessConsentsContext :
    IConsentContext<AccountAccessConsentRequest, AccountAccessConsentCreateResponse,
        AccountAccessConsentCreateResponse>,
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

internal class AccountAccessConsentsConsentContext :
    IAccountAccessConsentsContext
{
    private readonly ISharedContext _sharedContext;

    public AccountAccessConsentsConsentContext(ISharedContext sharedContext)
    {
        _sharedContext = sharedContext;
        var clientAccessTokenGet = new ClientAccessTokenGet(
            sharedContext.TimeProvider,
            new GrantPost(
                _sharedContext.ApiClient,
                _sharedContext.Instrumentation,
                _sharedContext.MemoryCache,
                _sharedContext.TimeProvider),
            sharedContext.Instrumentation,
            sharedContext.MemoryCache,
            sharedContext.EncryptionKeyInfo);
        var accountAccessConsentCommon = new AccountAccessConsentCommon(
            _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
            _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAccessToken>(),
            _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentRefreshToken>(),
            _sharedContext.Instrumentation,
            _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
            _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
            _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
            _sharedContext.DbService.GetDbMethods());
        var accountAccessConsentOperations = new AccountAccessConsentOperations(
            sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
            sharedContext.DbService.GetDbMethods(),
            sharedContext.TimeProvider,
            sharedContext.Instrumentation,
            sharedContext.ApiVariantMapper,
            sharedContext.BankProfileService,
            sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
            sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
            sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
            _sharedContext.ObWacCertificateMethods,
            _sharedContext.ObSealCertificateMethods,
            clientAccessTokenGet,
            accountAccessConsentCommon);
        CreateObject = accountAccessConsentOperations;
        ReadObject = accountAccessConsentOperations;
        DeleteObject =
            new AccountAccessConsentDelete(
                sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
                sharedContext.DbService.GetDbMethods(),
                sharedContext.TimeProvider,
                sharedContext.Instrumentation,
                _sharedContext.BankProfileService,
                sharedContext.ObSealCertificateMethods,
                sharedContext.ObWacCertificateMethods,
                clientAccessTokenGet,
                new AccountAccessConsentCommon(
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAccessToken>(),
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentRefreshToken>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                    _sharedContext.DbService.GetDbMethods()));
        AuthContexts = new LocalEntityContext<AccountAccessConsentAuthContextPersisted,
            AccountAccessConsentAuthContextRequest,
            IAccountAccessConsentAuthContextPublicQuery,
            AccountAccessConsentAuthContextCreateResponse,
            AccountAccessConsentAuthContextReadResponse>(
            _sharedContext,
            new AccountAccessConsentAuthContext(
                _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAuthContextPersisted>(),
                _sharedContext.DbService.GetDbMethods(),
                _sharedContext.TimeProvider,
                _sharedContext.Instrumentation,
                _sharedContext.BankProfileService,
                _sharedContext.ObWacCertificateMethods,
                _sharedContext.ObSealCertificateMethods,
                clientAccessTokenGet,
                accountAccessConsentCommon,
                _sharedContext.ApiVariantMapper));
    }

    public ILocalEntityContext<AccountAccessConsentAuthContextRequest,
        IAccountAccessConsentAuthContextPublicQuery,
        AccountAccessConsentAuthContextCreateResponse,
        AccountAccessConsentAuthContextReadResponse> AuthContexts { get; }

    public IObjectRead<AccountAccessConsentCreateResponse, ConsentReadParams> ReadObject { get; }

    public IObjectCreate<AccountAccessConsentRequest, AccountAccessConsentCreateResponse, ConsentCreateParams>
        CreateObject { get; }

    public IObjectDelete<ConsentDeleteParams> DeleteObject { get; }
}
