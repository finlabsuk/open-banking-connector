// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public class BankClientProfileResponse : OpenBankingResponse
    {
        public BankClientProfileResponse(Model.Public.Response.BankClientProfile data)
            : this((IList<OpenBankingResponseMessage>) null, data)
        {
        }

        public BankClientProfileResponse(OpenBankingResponseMessage message,
            Model.Public.Response.BankClientProfile data)
            : this(new[] { message }, data)
        {
        }

        public BankClientProfileResponse(IList<OpenBankingResponseMessage> messages, Model.Public.Response.BankClientProfile data)
            : base(messages)
        {
            Data = data;
        }

        public Model.Public.Response.BankClientProfile Data { get; }
    }
}
