﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using DomesticVrpConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.
    DomesticVrpConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class DomesticVrpConsentAuthContext :
        AuthContext,
        ISupportsFluentDeleteLocal<DomesticVrpConsentAuthContext>
    {
        public Guid DomesticVrpConsentId { get; set; }

        // Parent consent (optional to avoid warning due to non-support of global query filter)
        [ForeignKey("DomesticVrpConsentId")]
        public DomesticVrpConsent DomesticVrpConsentNavigation { get; set; } = null!;
    }

    internal partial class DomesticVrpConsentAuthContext :
        ISupportsFluentLocalEntityPost<DomesticVrpConsentAuthContextRequest,
            DomesticVrpConsentAuthContextPostResponse>
    {
        public void Initialise(
            DomesticVrpConsentAuthContextRequest request,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            base.Initialise(Guid.NewGuid(), request.Name, createdBy, timeProvider);
            DomesticVrpConsentId = request.DomesticVrpConsentId;
            TokenEndpointResponse = new ReadWriteProperty<TokenEndpointResponse?>(
                null,
                timeProvider,
                createdBy);
        }

        public DomesticVrpConsentAuthContextPostResponse PublicPostResponse =>
            throw new NotImplementedException("Do not use; use customised version instead.");

        public DomesticVrpConsentAuthContextPostResponse PublicPostResponseCustomised(string authUrl) =>
            new DomesticVrpConsentAuthContextPostResponse(
                Id,
                Name,
                Created,
                CreatedBy,
                authUrl);
    }

    internal partial class DomesticVrpConsentAuthContext :
        ISupportsFluentLocalEntityGet<DomesticVrpConsentAuthContextResponse>
    {
        public DomesticVrpConsentAuthContextResponse PublicGetResponse =>
            new DomesticVrpConsentAuthContextResponse(
                Id,
                Name,
                Created,
                CreatedBy);
    }
}
