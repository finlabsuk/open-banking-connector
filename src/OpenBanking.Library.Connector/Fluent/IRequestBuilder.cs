// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public interface IRequestBuilder
    {
        /// <summary>
        ///     API for setting up banks in OBC including OAuth2 clients and functional APIs
        /// </summary>
        IClientRegistration ClientRegistration { get; }

        /// <summary>
        ///     API corresponding to UK Open Banking Payment Initiation functional API
        /// </summary>
        IPaymentInitiation PaymentInitiation { get; }

        /// <summary>
        ///     API corresponding to UK Open Banking Account and Transaction functional API
        /// </summary>
        IAccountAndTransaction AccountAndTransaction { get; }
    }
}
