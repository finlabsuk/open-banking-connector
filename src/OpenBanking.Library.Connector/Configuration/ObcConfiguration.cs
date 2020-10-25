// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Configuration
{
    public partial class ObcConfiguration
    {
        // List of software statement profile IDs to be used by OBC
        // seperated by spaces.
        // Software statement profiles must be available as secrets.
        public string SoftwareStatementProfileIds { get; set; } = "all";

        public string EnsureDbCreated { get; set; } = "false";

        public string DbProvider { get; set; } = "Sqlite";
    }

    // Computed methods.
    public partial class ObcConfiguration
    {
        public const string OpenBankingConnector = "OpenBankingConnector";
        public IEnumerable<string> ProcessedSoftwareStatementProfileIds => SoftwareStatementProfileIds.Split(' ');

        public bool ProcessedEnsureDbCreated => string.Equals(
            a: EnsureDbCreated,
            b: "true",
            comparisonType: StringComparison.OrdinalIgnoreCase);

        public DbProvider ProcessedDbProvider => DbProviderHelper.DbProvider(DbProvider);
    }
}
