// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal class DomesticVrpConsentRefreshToken :
    RefreshTokenEntity
{
    /// <summary>
    ///     Constructor. Ideally would set all fields (full state) of class but unfortunately having parameters which don't
    ///     directly map to properties causes an issue for EF Core. Thus this constructor should be followed by a call
    ///     to <see cref="RefreshTokenEntity.UpdateRefreshToken" />.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="reference"></param>
    /// <param name="isDeleted"></param>
    /// <param name="isDeletedModified"></param>
    /// <param name="isDeletedModifiedBy"></param>
    /// <param name="created"></param>
    /// <param name="createdBy"></param>
    /// <param name="domesticVrpConsentId"></param>
    public DomesticVrpConsentRefreshToken(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        Guid domesticVrpConsentId) : base(
        id,
        reference,
        isDeleted,
        isDeletedModified,
        isDeletedModifiedBy,
        created,
        createdBy)
    {
        DomesticVrpConsentId = domesticVrpConsentId;
    }

    // Parent consent
    public DomesticVrpConsent DomesticVrpConsentNavigation { get; } = null!;

    public Guid DomesticVrpConsentId { get; }
}
