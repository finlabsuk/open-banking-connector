// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public class BankClientProfileFluentResponse : OpenBankingResponse
    {
        public BankClientProfileFluentResponse(BankClientProfileResponse data)
            : this((IList<OpenBankingResponseMessage>) null, data)
        {
        }

        public BankClientProfileFluentResponse(OpenBankingResponseMessage message,
            BankClientProfileResponse data)
            : this(new[] { message }, data)
        {
        }

        public BankClientProfileFluentResponse(IList<OpenBankingResponseMessage> messages,
            BankClientProfileResponse data)
            : base(messages)
        {
            Data = data;
        }

        public BankClientProfileResponse Data { get; }
    }
}
