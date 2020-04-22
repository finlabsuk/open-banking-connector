// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public class OpenBankingSoftwareStatementResponse : FluentResponse
    {
        internal OpenBankingSoftwareStatementResponse(FluentResponseMessage message,
            SoftwareStatementProfileResponse data) : this(new[] { message }, data)
        {
        }

        internal OpenBankingSoftwareStatementResponse(IList<FluentResponseMessage> messages,
            SoftwareStatementProfileResponse data) : base(messages)
        {
            Data = data;
        }

        public SoftwareStatementProfileResponse Data { get; }
    }
}
