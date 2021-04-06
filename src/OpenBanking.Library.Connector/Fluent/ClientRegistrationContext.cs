// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public interface IClientRegistrationContext
    {
        /// <summary>
        ///     API for Bank object which is the base object for a bank and is parent to BankRegistration and BankProfile objects
        /// </summary>
        ILocalEntityContext<Bank, BankPostResponse, IBankPublicQuery, BankGetLocalResponse> Banks { get; }

        /// <summary>
        ///     API for BankProfile object which adds configures functional API endpoints for a BankRegistration
        /// </summary>
        ILocalEntityContext<BankApiInformation, BankApiInformationPostResponse,
                IBankApiInformationPublicQuery, BankApiInformationGetLocalResponse>
            BankApiInformationObjects { get; }

        /// <summary>
        ///     API for BankRegistration object which corresponds to an OAuth2 client registration with a bank (represented by a
        ///     Bank object)
        /// </summary>
        ILocalEntityContext<BankRegistration, BankRegistrationPostResponse, IBankRegistrationPublicQuery,
                BankRegistrationGetLocalResponse>
            BankRegistrations { get; }
    }

    internal class ClientRegistrationContext : IClientRegistrationContext
    {
        private readonly ISharedContext _sharedContext;

        public ClientRegistrationContext(ISharedContext sharedContext)
        {
            _sharedContext = sharedContext;
        }

        public ILocalEntityContext<Bank, BankPostResponse, IBankPublicQuery, BankGetLocalResponse> Banks =>
            new LocalEntityContext<Models.Persistent.Bank, Bank, BankPostResponse, IBankPublicQuery, BankGetLocalResponse>(
                _sharedContext);

        public ILocalEntityContext<BankRegistration, BankRegistrationPostResponse, IBankRegistrationPublicQuery,
                BankRegistrationGetLocalResponse>
            BankRegistrations =>
            new LocalEntityContext<Models.Persistent.BankRegistration, BankRegistration,
                BankRegistrationPostResponse, IBankRegistrationPublicQuery, BankRegistrationGetLocalResponse>(
                _sharedContext);

        public ILocalEntityContext<BankApiInformation, BankApiInformationPostResponse, IBankApiInformationPublicQuery,
                BankApiInformationGetLocalResponse>
            BankApiInformationObjects =>
            new LocalEntityContext<Models.Persistent.BankApiInformation, BankApiInformation,
                BankApiInformationPostResponse, IBankApiInformationPublicQuery, BankApiInformationGetLocalResponse>(
                _sharedContext);
    }
}
