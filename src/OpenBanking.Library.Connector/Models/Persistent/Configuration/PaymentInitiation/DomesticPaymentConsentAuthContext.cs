﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.PaymentInitiation;

internal class
    DomesticPaymentConsentAuthContext : AuthContextConfig<
        Persistent.PaymentInitiation.DomesticPaymentConsentAuthContext>
{
    public DomesticPaymentConsentAuthContext(
        DbProvider dbProvider,
        bool supportsGlobalQueryFilter,
        Formatting jsonFormatting) : base(dbProvider, supportsGlobalQueryFilter, jsonFormatting) { }

    public override void Configure(
        EntityTypeBuilder<Persistent.PaymentInitiation.DomesticPaymentConsentAuthContext> builder)
    {
        base.Configure(builder);

        // Top-level property info: read-only, JSON conversion, etc
        builder.Property(e => e.DomesticPaymentConsentId)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
    }
}
