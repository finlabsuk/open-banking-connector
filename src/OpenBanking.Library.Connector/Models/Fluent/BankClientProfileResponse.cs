// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public class BankClientProfileResponse : OpenBankingResponse
    {
        public BankClientProfileResponse(BankClientProfile data)
            : this((IList<OpenBankingResponseMessage>) null, data)
        {
        }

        public BankClientProfileResponse(OpenBankingResponseMessage message,
            BankClientProfile data)
            : this(new[] { message }, data)
        {
        }

        public BankClientProfileResponse(IList<OpenBankingResponseMessage> messages, BankClientProfile data)
            : base(messages)
        {
            Data = data;
        }

        public BankClientProfile Data { get; }
    }
}
