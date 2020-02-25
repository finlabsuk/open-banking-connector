// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public class ClientProfileResponse : OpenBankingResponse
    {
        public ClientProfileResponse(OpenBankingClientProfileResponse data)
            : this((IList<OpenBankingResponseMessage>) null, data)
        {
        }

        public ClientProfileResponse(OpenBankingResponseMessage message,
            OpenBankingClientProfileResponse data)
            : this(new[] { message }, data)
        {
        }

        public ClientProfileResponse(IList<OpenBankingResponseMessage> messages, OpenBankingClientProfileResponse data)
            : base(messages)
        {
            Data = data;
        }

        public OpenBankingClientProfileResponse Data { get; }
    }
}
