﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public class AuthorisationCallbackDataContext
    {
        internal AuthorisationCallbackDataContext(ISharedContext context)
        {
            Context = context;
        }

        internal ISharedContext Context { get; }

        internal AuthorisationCallbackData Data { get; set; }

        internal string ResponseMode { get; set; }

        internal AuthorisationCallbackPayload Response { get; set; }
    }
}
