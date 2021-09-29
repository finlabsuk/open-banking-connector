// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.VariableRecurringPayments
{
    internal class DomesticVrp : Base<Persistent.VariableRecurringPayments.DomesticVrp>
    {
        private readonly Formatting _formatting;

        public DomesticVrp(Formatting formatting)
        {
            _formatting = formatting;
        }

        public override void Configure(EntityTypeBuilder<Persistent.VariableRecurringPayments.DomesticVrp> builder)
        {
            base.Configure(builder);

            // Top-level property info: read-only, JSON conversion, etc
            builder.Property(e => e.DomesticPaymentConsentId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.BankApiRequest)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v, _formatting),
                    v =>
                        JsonConvert.DeserializeObject<PaymentInitiationModelsPublic.OBWriteDomestic2>(v)!)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

            // Second-level property info
            builder.OwnsOne(
                p => p.BankApiResponse,
                od =>
                {
                    od.Property(e => e.Data)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v, _formatting),
                            v =>
                                JsonConvert
                                    .DeserializeObject<PaymentInitiationModelsPublic.OBWriteDomesticResponse5>(v)!)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.Property(e => e.Modified)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.Property(e => e.ModifiedBy)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                });
            builder.Navigation(p => p.BankApiResponse).IsRequired();
        }
    }
}
