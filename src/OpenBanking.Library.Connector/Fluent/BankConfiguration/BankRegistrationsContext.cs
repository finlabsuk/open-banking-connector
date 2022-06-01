// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration;
using BankRegistrationPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;
using BankPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.Bank;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.BankConfiguration
{
    public interface IBankRegistrationsContext :
        ICreateContext<BankRegistration, BankRegistrationResponse>,
        IReadLocalContext<IBankRegistrationPublicQuery, BankRegistrationResponse>,
        IReadContext<BankRegistrationResponse>,
        IDeleteLocalContext,
        IDeleteBankRegistrationContext { }

    internal interface IBankRegistrationsContextInternal :
        IBankRegistrationsContext,
        ICreateContextInternal<BankRegistration, BankRegistrationResponse>,
        IReadLocalContextInternal<IBankRegistrationPublicQuery, BankRegistrationResponse>,
        IReadContextInternal<BankRegistrationResponse>,
        IDeleteBankRegistrationContextInternal { }

    internal class BankRegistrationsContextInternal :
        ObjectContextBase<BankRegistrationPersisted>,
        IBankRegistrationsContextInternal
    {
        public BankRegistrationsContextInternal(ISharedContext sharedContext) : base(sharedContext)
        {
            ReadLocalObject =
                new LocalEntityGet<BankRegistrationPersisted, IBankRegistrationPublicQuery,
                    BankRegistrationResponse>(
                    sharedContext.DbService.GetDbEntityMethodsClass<BankRegistrationPersisted>(),
                    sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    sharedContext.TimeProvider,
                    sharedContext.SoftwareStatementProfileCachedRepo,
                    sharedContext.Instrumentation);
            ReadObject = new BankRegistrationGet(
                sharedContext.DbService.GetDbEntityMethodsClass<BankRegistrationPersisted>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation,
                sharedContext.ApiVariantMapper);
            DeleteObject = new BankRegistrationDelete(
                sharedContext.DbService.GetDbEntityMethodsClass<BankRegistrationPersisted>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation,
                sharedContext.BankProfileDefinitions);
            CreateObject = new BankRegistrationPost(
                sharedContext.DbService.GetDbEntityMethodsClass<BankRegistrationPersisted>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation,
                sharedContext.ApiVariantMapper,
                sharedContext.ApiClient,
                sharedContext.DbService.GetDbEntityMethodsClass<BankPersisted>(),
                sharedContext.BankProfileDefinitions);
        }

        public IObjectReadLocal<IBankRegistrationPublicQuery, BankRegistrationResponse> ReadLocalObject { get; }

        public IObjectCreate<BankRegistration, BankRegistrationResponse> CreateObject { get; }

        public IObjectDelete<BankRegistrationDeleteParams> DeleteObject { get; }
        public IObjectRead<BankRegistrationResponse> ReadObject { get; }
    }
}
