// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;

public interface IDomesticPaymentConsentAuthContextPublicQuery : IEntityBaseQuery
{
    public Guid DomesticPaymentConsentId { get; }
}

/// <summary>
///     Response to Read requests.
/// </summary>
public class DomesticPaymentConsentAuthContextReadResponse : EntityBaseResponse,
    IDomesticPaymentConsentAuthContextPublicQuery
{
    public required string State { get; init; }

    public required Guid DomesticPaymentConsentId { get; init; }
}

/// <summary>
///     Response to create requests.
/// </summary>
public class
    DomesticPaymentConsentAuthContextCreateResponse : DomesticPaymentConsentAuthContextReadResponse
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
