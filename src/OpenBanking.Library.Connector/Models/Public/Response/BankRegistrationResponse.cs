// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.DynamicClientRegistration.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    public interface IBankRegistrationPublicQuery
    {
        Guid Id { get; }

        ClientRegistrationModelsPublic.OBClientRegistration1 OBClientRegistrationRequest { get; }

        Guid BankId { get; }
    }

    /// <summary>
    ///     Respnose to GetLocal
    /// </summary>
    public class BankRegistrationGetLocalResponse : IBankRegistrationPublicQuery
    {
        internal BankRegistrationGetLocalResponse(
            Guid id,
            ClientRegistrationModelsPublic.OBClientRegistration1 bankClientRegistrationRequest,
            Guid bankId)
        {
            Id = id;
            OBClientRegistrationRequest = bankClientRegistrationRequest;
            BankId = bankId;
        }

        public Guid Id { get; }

        public ClientRegistrationModelsPublic.OBClientRegistration1 OBClientRegistrationRequest { get; }
        public Guid BankId { get; }
    }

    /// <summary>
    ///     Response to Post
    /// </summary>
    public class BankRegistrationPostResponse : BankRegistrationGetLocalResponse
    {
        internal BankRegistrationPostResponse(
            Guid id,
            ClientRegistrationModelsPublic.OBClientRegistration1 bankClientRegistrationRequest,
            Guid bankId) : base(id, bankClientRegistrationRequest, bankId) { }
    }
}
