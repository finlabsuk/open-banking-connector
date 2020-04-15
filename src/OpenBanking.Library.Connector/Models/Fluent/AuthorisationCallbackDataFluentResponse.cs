// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public class AuthorisationCallbackDataFluentResponse : FluentResponse
    {
        public AuthorisationCallbackDataFluentResponse(FluentResponseMessage message) : this(new[] { message })
        {
        }

        public AuthorisationCallbackDataFluentResponse(IList<FluentResponseMessage> messages) : base(messages)
        {
        }

        public AuthorisationCallbackDataFluentResponse(AuthorisationCallbackDataResponse data)
        {
            Data = data;
        }

        public AuthorisationCallbackDataResponse Data { get; }

    }
}
