// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using BankPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Bank;
using BankApiInformationPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankApiInformation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.BankConfiguration
{
    public interface IBankConfigurationContext
    {
        /// <summary>
        ///     API for Bank object which is the base object for a bank and is parent to BankRegistration and BankProfile objects
        /// </summary>
        ILocalEntityContext<Bank, IBankPublicQuery, BankResponse> Banks { get; }

        /// <summary>
        ///     API for BankProfile object which adds configures functional API endpoints for a BankRegistration
        /// </summary>
        ILocalEntityContext<BankApiInformation,
                IBankApiInformationPublicQuery, BankApiInformationResponse>
            BankApiInformationObjects { get; }

        /// <summary>
        ///     API for BankRegistration object which corresponds to an OAuth2 client registration with a bank (represented by a
        ///     Bank object)
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

        public ILocalEntityContext<BankApiInformation, IBankApiInformationPublicQuery,
                BankApiInformationResponse>
            BankApiInformationObjects =>
            new LocalEntityContext<BankApiInformationPersisted, BankApiInformation,
                IBankApiInformationPublicQuery, BankApiInformationResponse>(_sharedContext);
    }
}
