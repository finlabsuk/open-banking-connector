using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http
{
    /// <summary>
    /// Http Utils
    /// </summary>
    public static class HttpUtils
    {
        /// <summary>
        /// Parse a query string
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static NameValueCollection ParseQueryString(Uri uri)
        {
            return HttpUtility.ParseQueryString(uri.Query); 
        }
    }
}
