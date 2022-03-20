// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response
{
    /// <summary>
    ///     Messages from Open Banking Connector.
    /// </summary>
    public class HttpResponseMessages
    {
        /// <summary>
        ///     Information messages.
        /// </summary>
        [JsonProperty("info", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<string>? InformationMessages { get; set; }

        /// <summary>
        ///     Warning messages.
        /// </summary>
        [JsonProperty("warnings", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<string>? WarningMessages { get; set; }

        /// <summary>
        ///     Error messages.
        /// </summary>
        [JsonProperty("errors", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<string>? ErrorMessages { get; set; }
    }
}
