// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public class OpenBankingSoftwareStatementResponse : OpenBankingResponse
    {
        internal OpenBankingSoftwareStatementResponse(OpenBankingResponseMessage message,
            SoftwareStatementProfileResponse data) : this(new[] { message }, data)
        {
        }

        internal OpenBankingSoftwareStatementResponse(IList<OpenBankingResponseMessage> messages,
            SoftwareStatementProfileResponse data) : base(messages)
        {
            Data = data;
        }

        public SoftwareStatementProfileResponse Data { get; }
    }
}
