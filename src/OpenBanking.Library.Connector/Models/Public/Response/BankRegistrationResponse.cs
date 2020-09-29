// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    public interface IBankRegistrationPublicQuery
    {
        string Id { get; }

        OBClientRegistration1 BankClientRegistrationRequest { get; }
    }

    public class BankRegistrationResponse : IBankRegistrationPublicQuery
    {
        internal BankRegistrationResponse(string id, OBClientRegistration1 bankClientRegistrationRequest)
        {
            Id = id;
            BankClientRegistrationRequest = bankClientRegistrationRequest;
        }

        public string Id { get; }

        public OBClientRegistration1 BankClientRegistrationRequest { get; }
    }
}
