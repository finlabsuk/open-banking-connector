// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration
{
    internal class Bank : Base<Persistent.Bank>
    {
        private readonly Formatting _formatting;

        public Bank(Formatting formatting)
        {
            _formatting = formatting;
        }

        public override void Configure(EntityTypeBuilder<Persistent.Bank> builder)
        {
            base.Configure(builder);

            // Top-level read-only properties and foreign keys
            builder.Property(e => e.RegistrationScopeApiSet)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.IssuerUrl)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.FinancialId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.Name)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

            // Owned-level read-only properties and foreign keys
            builder.OwnsOne(
                navigationExpression: p => p.DefaultBankRegistrationId,
                buildAction: od =>
                {
                    od.Property(e => e.Data)
                        .IsRequired(false)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.Property(e => e.Modified)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.Property(e => e.ModifiedBy)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.HasOne<Persistent.BankRegistration>()
                        .WithOne()
                        .HasForeignKey<ReadWriteProperty<Guid?>>(b => b.Data);
                });
            builder.Navigation(p => p.DefaultBankRegistrationId).IsRequired();
            builder.OwnsOne(
                navigationExpression: p => p.StagingBankRegistrationId,
                buildAction: od =>
                {
                    od.Property(e => e.Data)
                        .IsRequired(false)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.Property(e => e.Modified)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.Property(e => e.ModifiedBy)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.HasOne<Persistent.BankRegistration>()
                        .WithOne()
                        .HasForeignKey<ReadWriteProperty<Guid?>>(b => b.Data);
                });
            builder.Navigation(p => p.StagingBankRegistrationId).IsRequired();
            builder.OwnsOne(
                navigationExpression: p => p.DefaultBankProfileId,
                buildAction: od =>
                {
                    od.Property(e => e.Data)
                        .IsRequired(false)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.Property(e => e.Modified)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.Property(e => e.ModifiedBy)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.HasOne<Persistent.BankApiInformation>()
                        .WithOne()
                        .HasForeignKey<ReadWriteProperty<Guid?>>(b => b.Data);
                });
            builder.Navigation(p => p.DefaultBankProfileId).IsRequired();
            builder.OwnsOne(
                navigationExpression: p => p.StagingBankProfileId,
                buildAction: od =>
                {
                    od.Property(e => e.Data)
                        .IsRequired(false)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.Property(e => e.Modified)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.Property(e => e.ModifiedBy)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.HasOne<Persistent.BankApiInformation>()
                        .WithOne()
                        .HasForeignKey<ReadWriteProperty<Guid?>>(b => b.Data);
                });
            builder.Navigation(p => p.StagingBankProfileId).IsRequired();
        }
    }
}
