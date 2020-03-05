// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation
{
    public class ApiProfileResponse : OpenBankingResponse
    {
        public ApiProfileResponse(ApiProfile data)
            : this((IList<OpenBankingResponseMessage>) null, data)
        {
        }

        public ApiProfileResponse(OpenBankingResponseMessage message,
            ApiProfile data)
            : this(new[] { message }, data)
        {
        }

        public ApiProfileResponse(IList<OpenBankingResponseMessage> messages, ApiProfile data)
            : base(messages)
        {
            Data = data;
        }

        public ApiProfile Data { get; }
    }
}
