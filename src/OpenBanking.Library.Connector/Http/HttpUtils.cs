// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Specialized;
using System.Web;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http
{
    /// <summary>
    ///     Http Utils
    /// </summary>
    public static class HttpUtils
    {
        /// <summary>
        ///     Parse a query string
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static NameValueCollection ParseQueryString(Uri uri)
        {
            return HttpUtility.ParseQueryString(uri.Query);
        }
    }
}
