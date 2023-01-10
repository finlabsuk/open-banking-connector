// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

public abstract class ConsentResponseBase : LocalObjectBaseResponse
{
    internal ConsentResponseBase(
        Guid id,
        DateTimeOffset created,
        string? createdBy,
        string? reference,
        IList<string>? warnings,
        Guid bankRegistrationId,
        string externalApiId,
        string? externalApiUserId,
        DateTimeOffset authContextModified,
        string? authContextModifiedBy) : base(id, created, createdBy, reference)
    {
        Warnings = warnings;
        BankRegistrationId = bankRegistrationId;
        ExternalApiId = externalApiId;
        ExternalApiUserId = externalApiUserId;
        AuthContextModified = authContextModified;
        AuthContextModifiedBy = authContextModifiedBy;
    }

    /// <summary>
    ///     Optional list of warning messages from Open Banking Connector.
    /// </summary>
    public IList<string>? Warnings { get; }

    /// <summary>
    ///     ID of associated BankRegistration object
    /// </summary>
    public Guid BankRegistrationId { get; }

    /// <summary>
    ///     External (bank) API ID. This should be unique between objects created at the
    ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
    /// </summary>
    public string ExternalApiId { get; }

    /// <summary>
    ///     User ID at external API (bank) which may or may not be available via ID token "sub" claim. If retrieved from ID
    ///     token or supplied on object creation, it will be stored here.
    /// </summary>
    public string? ExternalApiUserId { get; }

    public DateTimeOffset AuthContextModified { get; }

    public string? AuthContextModifiedBy { get; }
}
