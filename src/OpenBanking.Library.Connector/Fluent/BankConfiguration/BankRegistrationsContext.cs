// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using BankRegistrationPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;
using BankPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.Bank;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.BankConfiguration;

public interface IBankRegistrationsContext :
    ICreateBankRegistrationContext<BankRegistration, BankRegistrationResponse>,
    IReadBankRegistrationContext<BankRegistrationResponse>,
    IDeleteBankRegistrationContext { }

internal interface IBankRegistrationsContextInternal :
    IBankRegistrationsContext,
    ICreateBankRegistrationContextInternal<BankRegistration, BankRegistrationResponse>,
    IReadBankRegistrationContextInternal<BankRegistrationResponse>,
    IDeleteBankRegistrationContextInternal { }

internal class BankRegistrationsContextInternal :
    IBankRegistrationsContextInternal
{
    public BankRegistrationsContextInternal(ISharedContext sharedContext)
    {
        var bankRegistrationOperations = new BankRegistrationOperations(
            sharedContext.DbService.GetDbEntityMethodsClass<BankRegistrationPersisted>(),
            sharedContext.DbService.GetDbSaveChangesMethodClass(),
            sharedContext.TimeProvider,
            sharedContext.SoftwareStatementProfileCachedRepo,
            sharedContext.Instrumentation,
            sharedContext.ApiVariantMapper,
            new OpenIdConfigurationRead(sharedContext.ApiClient),
            sharedContext.DbService.GetDbEntityMethodsClass<BankPersisted>(),
            sharedContext.BankProfileService,
            new GrantPost(
                sharedContext.ApiClient,
                sharedContext.Instrumentation,
                sharedContext.MemoryCache,
                sharedContext.TimeProvider));
        ReadObject = bankRegistrationOperations;
        DeleteObject = new BankRegistrationDelete(
            sharedContext.DbService.GetDbEntityMethodsClass<BankRegistrationPersisted>(),
            sharedContext.DbService.GetDbSaveChangesMethodClass(),
            sharedContext.TimeProvider,
            sharedContext.SoftwareStatementProfileCachedRepo,
            sharedContext.Instrumentation,
            sharedContext.BankProfileService,
            new GrantPost(
                sharedContext.ApiClient,
                sharedContext.Instrumentation,
                sharedContext.MemoryCache,
                sharedContext.TimeProvider));
        CreateObject = bankRegistrationOperations;
    }

    public IObjectCreate<BankRegistration, BankRegistrationResponse, BankRegistrationCreateParams> CreateObject { get; }

    public IObjectDelete<BankRegistrationDeleteParams> DeleteObject { get; }
    public IObjectRead<BankRegistrationResponse, BankRegistrationReadParams> ReadObject { get; }
}
