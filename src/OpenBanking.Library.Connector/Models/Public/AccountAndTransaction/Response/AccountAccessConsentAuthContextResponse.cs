// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;

public interface IAccountAccessConsentAuthContextPublicQuery : IBaseQuery
{
    public Guid AccountAccessConsentId { get; }
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
        Guid accountAccessConsentId) : base(id, created, createdBy, reference)
    {
        AccountAccessConsentId = accountAccessConsentId;
    }

    /// <summary>
    ///     Optional list of warning messages from Open Banking Connector.
    /// </summary>
    public IList<string>? Warnings { get; set; }

    public Guid AccountAccessConsentId { get; }
}

/// <summary>
///     Response to Create requests.
/// </summary>
public class AccountAccessConsentAuthContextCreateResponse : AccountAccessConsentAuthContextReadResponse
{
    internal AccountAccessConsentAuthContextCreateResponse(
        Guid id,
        DateTimeOffset created,
        string? createdBy,
        string? reference,
        Guid accountAccessConsentId,
        string authUrl,
        string state) : base(id, created, createdBy, reference, accountAccessConsentId)
    {
        AuthUrl = authUrl;
        State = state;
    }

    public string AuthUrl { get; }

    public string State { get; }
}
