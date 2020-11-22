// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public interface IClientRegistration
    {
        /// <summary>
        ///     API for Bank object which is the base object for a bank and is parent to BankRegistration and BankProfile objects
        /// </summary>
        IFluentContextLocalEntity<Bank, BankResponse, IBankPublicQuery> Banks { get; }

        /// <summary>
        ///     API for BankProfile object which adds configures functional API endpoints for a BankRegistration
        /// </summary>
        IFluentContextLocalEntity<BankApiInformation, BankApiInformationResponse, IBankApiInformationPublicQuery>
            BankApiInformationObjects { get; }

        /// <summary>
        ///     API for BankRegistration object which corresponds to an OAuth2 client registration with a bank (represented by a
        ///     Bank object)
        /// </summary>
        IFluentContextLocalEntity<BankRegistration, BankRegistrationResponse, IBankRegistrationPublicQuery>
            BankRegistrations { get; }
    }
}
