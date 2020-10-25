// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration
{
    internal class BankProfile : Base<Persistent.BankProfile>
    {
        private readonly Formatting _formatting;

        public BankProfile(Formatting formatting)
        {
            _formatting = formatting;
        }

        public override void Configure(EntityTypeBuilder<Persistent.BankProfile> builder)
        {
            base.Configure(builder);

            // Top-level property info: read-only, JSON conversion, etc
            builder.Property(e => e.BankId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.PaymentInitiationApi)
                .HasConversion(
                    convertToProviderExpression: v => JsonConvert.SerializeObject(v, _formatting),
                    convertFromProviderExpression: v =>
                        JsonConvert.DeserializeObject<PaymentInitiationApi>(v))
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

            // Top-level foreign keys
            builder
                .HasOne<Persistent.Bank>()
                .WithMany()
                .HasForeignKey(p => p.BankId);
        }
    }
}
