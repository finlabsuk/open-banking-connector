// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using OBClientRegistration =
    FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models.OBClientRegistration1;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    public interface IBankRegistrationPublicQuery
    {
        Guid Id { get; }

        OBClientRegistration OBClientRegistrationRequest { get; }

        Guid BankId { get; }
    }

    public class BankRegistrationResponse : IBankRegistrationPublicQuery
    {
        internal BankRegistrationResponse(Guid id, OBClientRegistration bankClientRegistrationRequest, Guid bankId)
        {
            Id = id;
            OBClientRegistrationRequest = bankClientRegistrationRequest;
            BankId = bankId;
        }

        public Guid Id { get; }

        public OBClientRegistration OBClientRegistrationRequest { get; }
        public Guid BankId { get; }
    }
}
