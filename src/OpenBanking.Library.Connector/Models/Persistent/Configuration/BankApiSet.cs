﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration
{
    internal class BankApiSet : Base<Persistent.BankApiSet>
    {
        private readonly Formatting _formatting;

        public BankApiSet(Formatting formatting)
        {
            _formatting = formatting;
        }

        public override void Configure(EntityTypeBuilder<Persistent.BankApiSet> builder)
        {
            base.Configure(builder);

            // Top-level property info: read-only, JSON conversion, etc
            builder.Property(e => e.BankId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.PaymentInitiationApi)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v, _formatting),
                    v =>
                        JsonConvert.DeserializeObject<PaymentInitiationApi>(v))
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.VariableRecurringPaymentsApi)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v, _formatting),
                    v =>
                        JsonConvert.DeserializeObject<VariableRecurringPaymentsApi>(v))
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.Name)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        }
    }
}
