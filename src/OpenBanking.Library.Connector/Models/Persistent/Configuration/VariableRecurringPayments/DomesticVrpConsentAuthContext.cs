// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.VariableRecurringPayments
{
    internal class
        DomesticVrpConsentAuthContext : Base<Persistent.VariableRecurringPayments.DomesticVrpConsentAuthContext>
    {
        private readonly Formatting _formatting;

        public DomesticVrpConsentAuthContext(Formatting formatting)
        {
            _formatting = formatting;
        }

        public override void Configure(
            EntityTypeBuilder<Persistent.VariableRecurringPayments.DomesticVrpConsentAuthContext> builder)
        {
            base.Configure(builder);

            // Top-level property info: read-only, JSON conversion, etc
            builder.Property(e => e.DomesticVrpConsentId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

            // Second-level property info and foreign keys
            builder.OwnsOne(
                p => p.TokenEndpointResponse,
                od =>
                {
                    od.Property(e => e.Data)
                        .IsRequired(false)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v, _formatting),
                            v =>
                                JsonConvert.DeserializeObject<TokenEndpointResponse>(v))
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.Property(e => e.Modified)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.Property(e => e.ModifiedBy)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                });
            builder.Navigation(p => p.TokenEndpointResponse).IsRequired();
        }
    }
}
