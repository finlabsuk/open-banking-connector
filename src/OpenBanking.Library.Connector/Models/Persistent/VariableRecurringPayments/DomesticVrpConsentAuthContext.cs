// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal partial class DomesticVrpConsentAuthContext :
    AuthContext,
    IDomesticVrpConsentAuthContextPublicQuery
{
    public DomesticVrpConsentAuthContext(
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
        Guid domesticVrpConsentId) : base(
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
        DomesticVrpConsentId = domesticVrpConsentId;
    }

    // Parent consent
    public DomesticVrpConsent DomesticVrpConsentNavigation { get; } = null!;

    public Guid DomesticVrpConsentId { get; }
}

internal partial class DomesticVrpConsentAuthContext :
    ISupportsFluentLocalEntityGet<DomesticVrpConsentAuthContextReadResponse>
{
    public DomesticVrpConsentAuthContextReadResponse PublicGetLocalResponse =>
        new()
        {
            Id = Id,
            Created = Created,
            CreatedBy = CreatedBy,
            Reference = Reference,
            State = State,
            DomesticVrpConsentId = DomesticVrpConsentId
        };
}
