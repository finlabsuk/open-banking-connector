// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public class ClientRegistration : IClientRegistration
    {
        private readonly ISharedContext _sharedContext;

        public ClientRegistration(ISharedContext sharedContext)
        {
            _sharedContext = sharedContext;
        }

        public IFluentContextLocalEntity<Bank, BankResponse, IBankPublicQuery> Banks =>
            new FluentContext<Models.Persistent.Bank, Bank, BankResponse, IBankPublicQuery>(_sharedContext);

        public IFluentContextLocalEntity<BankRegistration, BankRegistrationResponse, IBankRegistrationPublicQuery>
            BankRegistrations =>
            new FluentContext<Models.Persistent.BankRegistration, BankRegistration, BankRegistrationResponse,
                IBankRegistrationPublicQuery>(_sharedContext);

        public IFluentContextLocalEntity<BankProfile, BankProfileResponse, IBankProfilePublicQuery> BankProfiles =>
            new FluentContext<Models.Persistent.BankProfile, BankProfile, BankProfileResponse, IBankProfilePublicQuery>(
                _sharedContext);
    }
}
