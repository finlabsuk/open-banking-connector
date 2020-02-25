// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Configuration
{
    // DB provider-independent DB context
    public abstract class BaseDbContext : DbContext
    {
        protected BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<SoftwareStatementProfile> SoftwareStatementProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Specify fields to be stored as JSON
            modelBuilder
                .Entity<SoftwareStatementProfile>(c =>
                {
                    c
                        .OwnsOne(e => e.SoftwareStatementPayload)
                        .Property(e => e.SoftwareRedirectUris)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v),
                            v => JsonConvert.DeserializeObject<string[]>(v)
                        );
                });

            modelBuilder
                .Entity<SoftwareStatementProfile>(c =>
                {
                    c
                        .OwnsOne(e => e.SoftwareStatementPayload)
                        .Property(e => e.SoftwareRoles)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v),
                            v => JsonConvert.DeserializeObject<string[]>(v)
                        );
                });
        }
    }
}
