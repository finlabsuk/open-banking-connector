// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;

public interface IAccountAccessConsentAuthContextPublicQuery : IEntityBaseQuery
{
    public Guid AccountAccessConsentId { get; }

    public string State { get; }
}

/// <summary>
///     Response to Read requests.
/// </summary>
public class AccountAccessConsentAuthContextReadResponse : EntityBaseResponse,
    IAccountAccessConsentAuthContextPublicQuery
{
    public required Guid AccountAccessConsentId { get; init; }

    public required string State { get; init; }
}

/// <summary>
///     Response to create requests.
/// </summary>
public class AccountAccessConsentAuthContextCreateResponse : AccountAccessConsentAuthContextReadResponse
{
    /// <summary>
    ///     Time-sensitive URL to enable end-user authentication via website or mobile app
    /// </summary>
    public required string AuthUrl { get; init; }

    /// <summary>
    ///     App session ID that can be checked when processing post-auth redirect
    /// </summary>
    public required string AppSessionId { get; init; }
}
