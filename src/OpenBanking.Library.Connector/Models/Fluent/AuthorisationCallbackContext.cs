// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public class AuthorisationCallbackContext
    {
        internal AuthorisationCallbackContext(OpenBankingContext context)
        {
            Context = context;
        }

        internal OpenBankingContext Context { get; }

        internal AuthorisationCallbackData Data { get; set; }
    }
}
