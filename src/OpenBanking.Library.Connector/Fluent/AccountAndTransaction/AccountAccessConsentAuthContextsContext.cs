// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
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

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.AccountAndTransaction;

public interface IAccountAccessConsentAuthContextsContext :
    ICreateLocalContext<AccountAccessConsentAuthContextRequest, AccountAccessConsentAuthContextCreateResponse>,
    IReadLocal2Context<AccountAccessConsentAuthContextReadResponse> { }

internal class AccountAccessConsentAuthContextsContext :
    IAccountAccessConsentAuthContextsContext
{
    private readonly ISharedContext _sharedContext;

    public AccountAccessConsentAuthContextsContext(ISharedContext sharedContext)
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
        var accountAccessConsentAuthContextOperations = new AccountAccessConsentAuthContextOperations(
            _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAuthContextPersisted>(),
            _sharedContext.DbService.GetDbMethods(),
            _sharedContext.TimeProvider,
            _sharedContext.Instrumentation,
            _sharedContext.BankProfileService,
            _sharedContext.ObWacCertificateMethods,
            _sharedContext.ObSealCertificateMethods,
            clientAccessTokenGet,
            accountAccessConsentCommon,
            _sharedContext.ApiVariantMapper);
        CreateLocalObject = accountAccessConsentAuthContextOperations;
        ReadLocalObject = accountAccessConsentAuthContextOperations;
    }

    public IObjectCreate<AccountAccessConsentAuthContextRequest, AccountAccessConsentAuthContextCreateResponse,
        LocalCreateParams> CreateLocalObject { get; }

    public IObjectRead<AccountAccessConsentAuthContextReadResponse, LocalReadParams> ReadLocalObject { get; }
}
