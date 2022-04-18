﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.PaymentInitiation
{
    internal class DomesticPaymentConsent : BaseConsentConfig<Persistent.PaymentInitiation.DomesticPaymentConsent>
    {
        public DomesticPaymentConsent(bool supportsGlobalQueryFilter, Formatting jsonFormatting) :
            base(
                supportsGlobalQueryFilter,
                jsonFormatting) { }

        public override void Configure(EntityTypeBuilder<Persistent.PaymentInitiation.DomesticPaymentConsent> builder)
        {
            base.Configure(builder);

            // Top-level property info: read-only, JSON conversion, etc
            builder.Property(e => e.Id)
                .HasColumnOrder(1);
            builder.Property(e => e.BankRegistrationId)
                .HasColumnOrder(2)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.PaymentInitiationApiId)
                .HasColumnOrder(3)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.ExternalApiId)
                .HasColumnOrder(4)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.AccessTokenAccessToken)
                .HasColumnOrder(5);
            builder.Property(e => e.AccessTokenExpiresIn)
                .HasColumnOrder(6);
            builder.Property(e => e.AccessTokenRefreshToken)
                .HasColumnOrder(7);
            builder.Property(e => e.AccessTokenModified)
                .HasColumnOrder(8);
            builder.Property(e => e.AccessTokenModifiedBy)
                .HasColumnOrder(9);
        }
    }
}
