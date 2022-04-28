﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
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
        public DomesticPaymentConsent(DbProvider dbProvider, bool supportsGlobalQueryFilter, Formatting jsonFormatting)
            : base(dbProvider, supportsGlobalQueryFilter, jsonFormatting) { }

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
            builder.Property("_accessTokenAccessToken")
                .HasColumnOrder(5);
            builder.Property("_accessTokenExpiresIn")
                .HasColumnOrder(6);
            builder.Property("_accessTokenRefreshToken")
                .HasColumnOrder(7);
            builder.Property("_accessTokenModified")
                .HasColumnOrder(8);
            builder.Property("_accessTokenModifiedBy")
                .HasColumnOrder(9);
        }
    }
}
