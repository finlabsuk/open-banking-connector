// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Configuration
{
    public class ObcConfiguration
    {
        public const string OpenBankingConnector = "OpenBankingConnector";

        public string DefaultCurrency { get; set; }
        public string SoftwareId { get; set; }
        public string EnsureDbCreated { get; set; }
    }
}
