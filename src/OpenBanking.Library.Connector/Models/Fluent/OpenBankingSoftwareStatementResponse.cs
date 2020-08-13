// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public class OpenBankingSoftwareStatementResponse : FluentResponse
    {
        //internal
        public OpenBankingSoftwareStatementResponse(
            FluentResponseMessage message,
            SoftwareStatementProfileResponse data) : this(messages: new[] { message }, data: data) { }

        //internal
        public OpenBankingSoftwareStatementResponse(
            IList<FluentResponseMessage> messages,
            SoftwareStatementProfileResponse data) : base(messages)
        {
            Data = data;
        }

        public SoftwareStatementProfileResponse Data { get; }
    }
}
