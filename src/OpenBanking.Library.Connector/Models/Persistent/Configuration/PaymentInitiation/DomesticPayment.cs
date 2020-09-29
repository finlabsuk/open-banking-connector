// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using OBWriteDomesticResponse =
    FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model.OBWriteDomesticResponse4;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.PaymentInitiation
{
    internal class DomesticPayment : Base<Persistent.PaymentInitiation.DomesticPayment>
    {
        private readonly Formatting _formatting;

        public DomesticPayment(Formatting formatting)
        {
            _formatting = formatting;
        }

        public override void Configure(EntityTypeBuilder<Persistent.PaymentInitiation.DomesticPayment> builder)
        {
            base.Configure(builder);

            // Top-level property info: read-only, JSON conversion, etc
            builder.Property(e => e.DomesticPaymentConsentId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.OBWriteDomesticResponse)
                .HasConversion(
                    convertToProviderExpression: v => JsonConvert.SerializeObject(v, _formatting),
                    convertFromProviderExpression: v =>
                        JsonConvert.DeserializeObject<OBWriteDomesticResponse>(v))
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        }
    }
}
