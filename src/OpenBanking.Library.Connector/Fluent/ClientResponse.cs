// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public class ClientResponse : OpenBankingResponse
    {
        public ClientResponse(BankClient data)
            : this((IList<OpenBankingResponseMessage>) null, data)
        {
        }

        public ClientResponse(OpenBankingResponseMessage message,
            BankClient data)
            : this(new[] { message }, data)
        {
        }

        public ClientResponse(IList<OpenBankingResponseMessage> messages, BankClient data)
            : base(messages)
        {
            Data = data;
        }

        public BankClient Data { get; }
    }
}
