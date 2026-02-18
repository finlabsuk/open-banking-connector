// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal partial class DomesticPaymentConsentAuthContext :
    AuthContext,
    IDomesticPaymentConsentAuthContextPublicQuery
{
    public DomesticPaymentConsentAuthContext(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        string state,
        string nonce,
        string? codeVerifier,
        string appSessionId,
        Guid domesticPaymentConsentId) : base(
        id,
        reference,
        isDeleted,
        isDeletedModified,
        isDeletedModifiedBy,
        created,
        createdBy,
        state,
        nonce,
        codeVerifier,
        appSessionId)
    {
        DomesticPaymentConsentId = domesticPaymentConsentId;
    }

    // Parent consent
    public DomesticPaymentConsent DomesticPaymentConsentNavigation { get; } = null!;

    public Guid DomesticPaymentConsentId { get; }
}

internal partial class DomesticPaymentConsentAuthContext :
    ISupportsFluentLocalEntityGet<DomesticPaymentConsentAuthContextReadResponse>
{
    public DomesticPaymentConsentAuthContextReadResponse PublicGetLocalResponse =>
        new()
        {
            Id = Id,
            Created = Created,
            CreatedBy = CreatedBy,
            Reference = Reference,
            State = State,
            DomesticPaymentConsentId = DomesticPaymentConsentId
        };
}
