// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.BankConfiguration
{
    internal class BankApiSet : Base<Persistent.BankConfiguration.BankApiSet>
    {
        public BankApiSet(bool supportsGlobalQueryFilter, Formatting jsonFormatting) :
            base(
                supportsGlobalQueryFilter,
                jsonFormatting) { }

        public override void Configure(EntityTypeBuilder<Persistent.BankConfiguration.BankApiSet> builder)
        {
            base.Configure(builder);

            // Top-level property info: read-only, JSON conversion, etc
            builder.Property(e => e.VariableRecurringPaymentsApiVersion)
                .HasConversion(new EnumToStringConverter<VariableRecurringPaymentsApiVersionEnum>())
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.VariableRecurringPaymentsApiBaseUrl)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.PaymentInitiationApiVersion)
                .HasConversion(new EnumToStringConverter<PaymentInitiationApiVersionEnum>())
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.PaymentInitiationApiBaseUrl)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.BankId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        }
    }
}
