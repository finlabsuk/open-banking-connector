// Licensed to Finnovation Labs Limited under one or more agreements.
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
        ISupportsFluentDeleteLocal<DomesticVrpConsentAuthContext>,
        IDomesticVrpConsentAuthContextPublicQuery
    {
        // Parent consent (optional to avoid warning due to non-support of global query filter)
        [ForeignKey("DomesticVrpConsentId")]
        public DomesticVrpConsent DomesticVrpConsentNavigation { get; set; } = null!;

        public Guid DomesticVrpConsentId { get; set; }
    }

    internal partial class DomesticVrpConsentAuthContext :
        ISupportsFluentLocalEntityPost<DomesticVrpConsentAuthContextRequest,
            DomesticVrpConsentAuthContextCreateLocalResponse, DomesticVrpConsentAuthContext>
    {
        public DomesticVrpConsentAuthContext() { }

        private DomesticVrpConsentAuthContext(
            Guid domesticVrpConsentId,
            ReadWriteProperty<TokenEndpointResponse?> tokenEndpointResponse,
            Guid id,
            string? name,
            string? createdBy,
            ITimeProvider timeProvider) : base(
            id,
            name,
            createdBy,
            timeProvider)
        {
            DomesticVrpConsentId = domesticVrpConsentId;
            TokenEndpointResponse = tokenEndpointResponse;
        }

        public DomesticVrpConsentAuthContext Create(
            DomesticVrpConsentAuthContextRequest request,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            var tokenEndpointResponse = new ReadWriteProperty<TokenEndpointResponse?>(
                null,
                timeProvider,
                createdBy);

            var output = new DomesticVrpConsentAuthContext(
                request.DomesticVrpConsentId,
                tokenEndpointResponse,
                Guid.NewGuid(),
                request.Name,
                createdBy,
                timeProvider);

            return output;
        }


        public DomesticVrpConsentAuthContextCreateLocalResponse PublicPostResponse =>
            throw new NotImplementedException("Do not use; use customised version instead.");

        public DomesticVrpConsentAuthContextCreateLocalResponse PublicPostResponseCustomised(string authUrl) =>
            new DomesticVrpConsentAuthContextCreateLocalResponse(
                Id,
                Name,
                Created,
                CreatedBy,
                DomesticVrpConsentId,
                authUrl);
    }

    internal partial class DomesticVrpConsentAuthContext :
        ISupportsFluentLocalEntityGet<DomesticVrpConsentAuthContextReadLocalResponse>
    {
        public DomesticVrpConsentAuthContextReadLocalResponse PublicGetResponse =>
            new DomesticVrpConsentAuthContextReadLocalResponse(
                Id,
                Name,
                Created,
                CreatedBy,
                DomesticVrpConsentId);
    }
}
