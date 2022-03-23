// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using BankPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Bank;
using BankApiSetPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankApiSet;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.BankConfiguration
{
    public interface IBankConfigurationContext
    {
        /// <summary>
        ///     API for Bank objects.
        ///     A Bank is the base object for a bank in Open Banking Connector and is parent to BankRegistration and BankApiSet
        ///     objects
        /// </summary>
        ILocalEntityContext<Bank, IBankPublicQuery, BankResponse, BankResponse> Banks { get; }

        /// <summary>
        ///     API for BankApiSet objects.
        ///     A BankApiSet specifies functional API(s) supported by a Bank. Multiple BankApiSets may be
        ///     created for the same bank.
        /// </summary>
        ILocalEntityContext<BankApiSet, IBankApiSetPublicQuery, BankApiSetResponse, BankApiSetResponse>
            BankApiSets { get; }

        /// <summary>
        ///     API for BankRegistration objects.
        ///     A BankRegistration corresponds to an OAuth2 client registration with a Bank. Multiple BankRegistrations may be
        ///     created for the same bank.
        /// </summary>
        IBankRegistrationsContext
            BankRegistrations { get; }
    }

    internal class BankConfigurationContext : IBankConfigurationContext
    {
        private readonly ISharedContext _sharedContext;

        public BankConfigurationContext(ISharedContext sharedContext)
        {
            _sharedContext = sharedContext;
        }

        public ILocalEntityContext<Bank, IBankPublicQuery, BankResponse, BankResponse> Banks =>
            new LocalEntityContextInternal<BankPersisted, Bank, IBankPublicQuery,
                BankResponse, BankResponse>(
                _sharedContext,
                new LocalEntityPost<BankPersisted, Bank, BankResponse>(
                    _sharedContext.DbService.GetDbEntityMethodsClass<BankPersisted>(),
                    _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    _sharedContext.TimeProvider,
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.Instrumentation));

        public IBankRegistrationsContext
            BankRegistrations =>
            new BankRegistrationsContextInternal(_sharedContext);

        public ILocalEntityContext<BankApiSet, IBankApiSetPublicQuery,
                BankApiSetResponse, BankApiSetResponse>
            BankApiSets =>
            new LocalEntityContextInternal<BankApiSetPersisted, BankApiSet,
                IBankApiSetPublicQuery, BankApiSetResponse, BankApiSetResponse>(
                _sharedContext,
                new LocalEntityPost<BankApiSetPersisted, BankApiSet, BankApiSetResponse>(
                    _sharedContext.DbService.GetDbEntityMethodsClass<BankApiSetPersisted>(),
                    _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    _sharedContext.TimeProvider,
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.Instrumentation));
    }
}
