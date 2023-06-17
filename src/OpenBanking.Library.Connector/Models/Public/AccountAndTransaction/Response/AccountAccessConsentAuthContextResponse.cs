// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;

public interface IAccountAccessConsentAuthContextPublicQuery : IBaseQuery
{
    public Guid AccountAccessConsentId { get; }

    public string State { get; }
}

/// <summary>
///     Response to Read requests.
/// </summary>
public class AccountAccessConsentAuthContextReadResponse : LocalObjectBaseResponse,
    IAccountAccessConsentAuthContextPublicQuery
{
    internal AccountAccessConsentAuthContextReadResponse(
        Guid id,
        DateTimeOffset created,
        string? createdBy,
        string? reference,
        IList<string>? warnings,
        Guid accountAccessConsentId,
        string state) : base(id, created, createdBy, reference)
    {
        Warnings = warnings;
        AccountAccessConsentId = accountAccessConsentId;
        State = state;
    }

    /// <summary>
    ///     Optional list of warning messages from Open Banking Connector.
    /// </summary>
    public IList<string>? Warnings { get; }

    public Guid AccountAccessConsentId { get; }

    public string State { get; }
}

/// <summary>
///     Response to create requests.
/// </summary>
public class AccountAccessConsentAuthContextCreateResponse : AccountAccessConsentAuthContextReadResponse
{
    internal AccountAccessConsentAuthContextCreateResponse(
        Guid id,
        DateTimeOffset created,
        string? createdBy,
        string? reference,
        IList<string>? warnings,
        Guid accountAccessConsentId,
        string state,
        string authUrl,
        string appSessionId) : base(id, created, createdBy, reference, warnings, accountAccessConsentId, state)
    {
        AuthUrl = authUrl;
        AppSessionId = appSessionId;
    }

    /// <summary>
    ///     Time-sensitive URL to enable end-user authentication via website or mobile app
    /// </summary>
    public string AuthUrl { get; }

    /// <summary>
    ///     App session ID that can be checked when processing post-auth redirect
    /// </summary>
    public string AppSessionId { get; }
}
