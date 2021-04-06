// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    /// Used for body of 400 or 500 series request errors
    public class Error
    {
        /// Error description
        public string Description { get; set; } = null!;
    }
}
