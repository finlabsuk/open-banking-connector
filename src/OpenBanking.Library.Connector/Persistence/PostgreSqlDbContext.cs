// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    // PostgreSql DB context
    public class PostgreSqlDbContext : BaseDbContext
    {
        public PostgreSqlDbContext(DbContextOptions<PostgreSqlDbContext> options) : base(options) { }

        // Use indenting to aid visualisation
        protected override Formatting JsonFormatting => Formatting.None;

        protected override DbProvider DbProvider => DbProvider.PostgreSql;
    }
}
