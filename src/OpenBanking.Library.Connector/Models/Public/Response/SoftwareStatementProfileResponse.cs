// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    /// Response to successful POST of software statement profile
    public class SoftwareStatementProfileResponse
    {
        public SoftwareStatementProfileResponse(string id)
        {
            Id = id;
        }

        /// ID used to uniquely identify object
        public string Id { get; set; }
    }
}
