﻿// Licensed to Finnovation Labs Limited under one or more agreements.
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
    internal class
        DomesticPaymentConsentAuthContext : Base<Persistent.PaymentInitiation.DomesticPaymentConsentAuthContext>
    {
        private readonly Formatting _formatting;

        public DomesticPaymentConsentAuthContext(Formatting formatting)
        {
            _formatting = formatting;
        }

        public override void Configure(
            EntityTypeBuilder<Persistent.PaymentInitiation.DomesticPaymentConsentAuthContext> builder)
        {
            base.Configure(builder);

            // Top-level property info: read-only, JSON conversion, etc
            builder.Property(e => e.DomesticPaymentConsentId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

            // Top-level foreign keys
            builder
                .HasOne<Persistent.PaymentInitiation.DomesticPaymentConsent>()
                .WithMany()
                .HasForeignKey(r => r.DomesticPaymentConsentId);

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
