// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
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
        ILocalEntityContext<Bank, IBankPublicQuery, BankResponse> Banks { get; }

        /// <summary>
        ///     API for BankApiSet objects.
        ///     A BankApiSet specifies functional API(s) supported by a Bank. Multiple BankApiSets may be
        ///     created for the same bank.
        /// </summary>
        ILocalEntityContext<BankApiSet,
                IBankApiSetPublicQuery, BankApiSetResponse>
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

        public ILocalEntityContext<Bank, IBankPublicQuery, BankResponse> Banks =>
            new LocalEntityContext<BankPersisted, Bank, IBankPublicQuery,
                BankResponse>(_sharedContext);

        public IBankRegistrationsContext
            BankRegistrations =>
            new BankRegistrationsContext(_sharedContext);

        public ILocalEntityContext<BankApiSet, IBankApiSetPublicQuery,
                BankApiSetResponse>
            BankApiSets =>
            new LocalEntityContext<BankApiSetPersisted, BankApiSet,
                IBankApiSetPublicQuery, BankApiSetResponse>(_sharedContext);
    }
}
