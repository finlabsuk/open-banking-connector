// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Management;

public interface IBankRegistrationsContext :
    ICreateBankRegistrationContext<BankRegistration, BankRegistrationResponse>,
    IReadBankRegistrationContext<BankRegistrationResponse>,
    IDeleteBankRegistrationContext { }

internal class BankRegistrationsContext :
    IBankRegistrationsContext
{
    public BankRegistrationsContext(ISharedContext sharedContext)
    {
        var grantPost = new GrantPost(
            sharedContext.ApiClient,
            sharedContext.Instrumentation,
            sharedContext.MemoryCache,
            sharedContext.TimeProvider);
        var clientAccessTokenGet = new ClientAccessTokenGet(
            sharedContext.TimeProvider,
            grantPost,
            sharedContext.Instrumentation,
            sharedContext.MemoryCache,
            sharedContext.EncryptionKeyInfo);
        var bankRegistrationOperations = new BankRegistrationOperations(
            sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
            sharedContext.DbService.GetDbMethods(),
            sharedContext.TimeProvider,
            sharedContext.Instrumentation,
            sharedContext.ApiVariantMapper,
            new OpenIdConfigurationRead(sharedContext.ApiClient),
            sharedContext.BankProfileService,
            sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
            sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
            sharedContext.DbService.GetDbEntityMethods<RegistrationAccessTokenEntity>(),
            sharedContext.ObWacCertificateMethods,
            sharedContext.ObSealCertificateMethods,
            clientAccessTokenGet,
            grantPost,
            sharedContext.EncryptionKeyInfo,
            sharedContext.SecretProvider);
        ReadObject = bankRegistrationOperations;
        DeleteObject = new BankRegistrationDelete(
            sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
            sharedContext.DbService.GetDbMethods(),
            sharedContext.TimeProvider,
            sharedContext.Instrumentation,
            sharedContext.BankProfileService,
            sharedContext.ObWacCertificateMethods,
            sharedContext.ObSealCertificateMethods,
            clientAccessTokenGet,
            sharedContext.EncryptionKeyInfo,
            sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
            sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
            sharedContext.DbService.GetDbEntityMethods<RegistrationAccessTokenEntity>());
        CreateObject = bankRegistrationOperations;
    }

    public IObjectCreate<BankRegistration, BankRegistrationResponse, BankRegistrationCreateParams> CreateObject { get; }

    public IObjectDelete<BankRegistrationDeleteParams> DeleteObject { get; }
    public IObjectRead<BankRegistrationResponse, BankRegistrationReadParams> ReadObject { get; }
}
