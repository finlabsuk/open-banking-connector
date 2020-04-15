// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public class BankClientProfileFluentResponse : FluentResponse
    {
        public BankClientProfileFluentResponse(BankClientProfileResponse data)
            : this((IList<FluentResponseMessage>) null, data)
        {
        }

        public BankClientProfileFluentResponse(FluentResponseMessage message,
            BankClientProfileResponse data)
            : this(new[] { message }, data)
        {
        }

        public BankClientProfileFluentResponse(IList<FluentResponseMessage> messages,
            BankClientProfileResponse data)
            : base(messages)
        {
            Data = data;
        }

        public BankClientProfileResponse Data { get; }
    }
}
