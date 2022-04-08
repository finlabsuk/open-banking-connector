// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal static class AuthContextAccessTokenGet
    {
        public static string GetAccessToken<TAuthContext>(IList<TAuthContext> input)
            where TAuthContext : AuthContext
        {
            // Get token
            List<TAuthContext> authContextsWithToken =
                input
                    .Where(x => x.AccessToken.Value1 != null)
                    .ToList();

            if (!authContextsWithToken.Any())
            {
                throw new InvalidOperationException("No token is available for Consent.");
            }

            TAuthContext authContext = authContextsWithToken
                .OrderByDescending(x => x.AccessToken.Modified)
                .First();
            return authContext.AccessToken.Value1!; // We already filtered out null entries above
        }
    }
}
