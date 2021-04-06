// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p6.PaymentInitiation.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.PaymentInitiation
{
    internal class DomesticPaymentConsent : Base<Persistent.PaymentInitiation.DomesticPaymentConsent>
    {
        private readonly Formatting _formatting;

        public DomesticPaymentConsent(Formatting formatting)
        {
            _formatting = formatting;
        }

        public override void Configure(EntityTypeBuilder<Persistent.PaymentInitiation.DomesticPaymentConsent> builder)
        {
            base.Configure(builder);

            // Top-level property info: read-only, JSON conversion, etc
            builder.Property(e => e.BankRegistrationId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.BankApiInformationId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.OBWriteDomesticConsent)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v, _formatting),
                    v =>
                        JsonConvert.DeserializeObject<PaymentInitiationModelsPublic.OBWriteDomesticConsent4>(v)!)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.OBWriteDomesticConsentResponse)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v, _formatting),
                    v =>
                        JsonConvert
                            .DeserializeObject<PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5>(v)!)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

            // Top-level foreign keys
            builder
                .HasOne<Persistent.BankRegistration>()
                .WithMany()
                .HasForeignKey(r => r.BankRegistrationId);
            builder
                .HasOne<Persistent.BankApiInformation>()
                .WithMany()
                .HasForeignKey(r => r.BankApiInformationId);

            // Second-level property info and foreign keys
            builder.OwnsOne(
                p => p.State,
                od =>
                {
                    od.Property(e => e.Data)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.Property(e => e.Modified)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    od.Property(e => e.ModifiedBy)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                });
            builder.Navigation(p => p.State).IsRequired();
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
