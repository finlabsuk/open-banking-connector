// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
[Index(nameof(State), IsUnique = true)]
[Index(nameof(Nonce), IsUnique = true)]
[Index(nameof(CodeVerifier), IsUnique = true)]
[Index(nameof(AppSessionId), IsUnique = true)]
internal class AuthContext : BaseEntity
{
    public AuthContext(
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
        string appSessionId) : base(
        id,
        reference,
        isDeleted,
        isDeletedModified,
        isDeletedModifiedBy,
        created,
        createdBy)
    {
        State = state ?? throw new ArgumentNullException(nameof(state));
        Nonce = nonce ?? throw new ArgumentNullException(nameof(nonce));
        CodeVerifier = codeVerifier;
        AppSessionId = appSessionId ?? throw new ArgumentNullException(nameof(appSessionId));
    }

    /// <summary>
    ///     OAuth2 "state".
    /// </summary>
    public string State { get; }

    /// <summary>
    ///     OpenID Connect "nonce".
    /// </summary>
    public string Nonce { get; }

    /// <summary>
    ///     Code verifier for PKCE.
    /// </summary>
    public string? CodeVerifier { get; }

    /// <summary>
    ///     App session ID.
    /// </summary>
    public string AppSessionId { get; }
}
