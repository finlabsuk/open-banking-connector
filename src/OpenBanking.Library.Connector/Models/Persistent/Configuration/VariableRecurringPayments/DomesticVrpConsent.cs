﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.VariableRecurringPayments
{
    internal class DomesticVrpConsent : BaseConsentConfig<Persistent.VariableRecurringPayments.DomesticVrpConsent>
    {
        public DomesticVrpConsent(bool supportsGlobalQueryFilter, Formatting jsonFormatting) :
            base(
                supportsGlobalQueryFilter,
                jsonFormatting) { }

        public override void Configure(
            EntityTypeBuilder<Persistent.VariableRecurringPayments.DomesticVrpConsent> builder)
        {
            base.Configure(builder);

            // Top-level property info: read-only, JSON conversion, etc
            builder.Property(e => e.Id)
                .HasColumnOrder(1);
            builder.Property(e => e.BankRegistrationId)
                .HasColumnOrder(2)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.VariableRecurringPaymentsApiId)
                .HasColumnOrder(3)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.ExternalApiId)
                .HasColumnOrder(4)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.AccessToken_AccessToken)
                .HasColumnOrder(5);
            builder.Property(e => e.AccessToken_ExpiresIn)
                .HasColumnOrder(6);
            builder.Property(e => e.AccessToken_RefreshToken)
                .HasColumnOrder(7);
            builder.Property(e => e.AccessTokenModified)
                .HasColumnOrder(8);
            builder.Property(e => e.AccessTokenModifiedBy)
                .HasColumnOrder(9);
        }
    }
}
