﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal partial class AccountAccessConsentAuthContext :
    AuthContext
{
    public AccountAccessConsentAuthContext(
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
        Guid accountAccessConsentId) : base(
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
        AccountAccessConsentId = accountAccessConsentId;
    }

    // Parent consent
    public AccountAccessConsent AccountAccessConsentNavigation { get; } = null!;

    public Guid AccountAccessConsentId { get; }
}

internal partial class AccountAccessConsentAuthContext :
    ISupportsFluentLocalEntityGet<AccountAccessConsentAuthContextReadResponse>
{
    public AccountAccessConsentAuthContextReadResponse PublicGetLocalResponse =>
        new()
        {
            Id = Id,
            Created = Created,
            CreatedBy = CreatedBy,
            Reference = Reference,
            AccountAccessConsentId = AccountAccessConsentId,
            State = State
        };
}
