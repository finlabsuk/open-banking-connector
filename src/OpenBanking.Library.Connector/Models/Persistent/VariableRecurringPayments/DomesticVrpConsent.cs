// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal class DomesticVrpConsent :
    BaseConsent,
    IDomesticVrpConsentPublicQuery
{
    public DomesticVrpConsent(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        string? accessTokenAccessToken,
        int accessTokenExpiresIn,
        DateTimeOffset accessTokenModified,
        string? accessTokenModifiedBy,
        string? accessTokenRefreshToken,
        Guid bankRegistrationId,
        string externalApiId,
        string? authContextState,
        string? authContextNonce,
        DateTimeOffset authContextModified,
        string? authContextModifiedBy,
        string? externalApiUserId,
        DateTimeOffset externalApiUserIdModified,
        string? externalApiUserIdModifiedBy,
        Guid? variableRecurringPaymentsApiId) : base(
        id,
        reference,
        isDeleted,
        isDeletedModified,
        isDeletedModifiedBy,
        created,
        createdBy,
        accessTokenAccessToken,
        accessTokenExpiresIn,
        accessTokenModified,
        accessTokenModifiedBy,
        accessTokenRefreshToken,
        bankRegistrationId,
        externalApiId,
        authContextState,
        authContextNonce,
        authContextModified,
        authContextModifiedBy,
        externalApiUserId,
        externalApiUserIdModified,
        externalApiUserIdModifiedBy)
    {
        VariableRecurringPaymentsApiId = variableRecurringPaymentsApiId;
    }

    [ForeignKey("VariableRecurringPaymentsApiId")]
    public VariableRecurringPaymentsApiEntity VariableRecurringPaymentsApiNavigation { get; set; } = null!;

    /// <summary>
    ///     Associated VariableRecurringPaymentsApi object
    /// </summary>
    public Guid? VariableRecurringPaymentsApiId { get; }

    /// <summary>
    ///     Associated auth contexts
    /// </summary>
    public IList<DomesticVrpConsentAuthContext> DomesticVrpConsentAuthContextsNavigation { get; private set; } =
        new List<DomesticVrpConsentAuthContext>();

    /// <summary>
    ///     Associated access tokens
    /// </summary>
    public IList<DomesticVrpConsentAccessToken> DomesticVrpConsentAccessTokensNavigation { get; } =
        new List<DomesticVrpConsentAccessToken>();

    /// <summary>
    ///     Associated refresh tokens
    /// </summary>
    public IList<DomesticVrpConsentRefreshToken> DomesticVrpConsentRefreshTokensNavigation { get; } =
        new List<DomesticVrpConsentRefreshToken>();

    public override AccessTokenEntity AddNewAccessToken(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy)
    {
        var domesticVrpConsentAccessToken =
            new DomesticVrpConsentAccessToken(
                id,
                reference,
                isDeleted,
                isDeletedModified,
                isDeletedModifiedBy,
                created,
                createdBy,
                Id);
        DomesticVrpConsentAccessTokensNavigation.Add(domesticVrpConsentAccessToken);
        return domesticVrpConsentAccessToken;
    }

    public override RefreshTokenEntity AddNewRefreshToken(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy)
    {
        var domesticVrpConsentRefreshToken =
            new DomesticVrpConsentRefreshToken(
                Guid.NewGuid(),
                null,
                false,
                isDeletedModified,
                isDeletedModifiedBy,
                created,
                createdBy,
                Id);
        DomesticVrpConsentRefreshTokensNavigation.Add(domesticVrpConsentRefreshToken);
        return domesticVrpConsentRefreshToken;
    }
}
