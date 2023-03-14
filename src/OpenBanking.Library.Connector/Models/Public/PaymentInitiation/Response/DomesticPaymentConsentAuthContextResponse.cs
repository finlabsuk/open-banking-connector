// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;

public interface IDomesticPaymentConsentAuthContextPublicQuery : IBaseQuery
{
    public Guid DomesticPaymentConsentId { get; }
}

/// <summary>
///     Response to Read requests.
/// </summary>
public class DomesticPaymentConsentAuthContextReadResponse : LocalObjectBaseResponse,
    IDomesticPaymentConsentAuthContextPublicQuery
{
    internal DomesticPaymentConsentAuthContextReadResponse(
        Guid id,
        DateTimeOffset created,
        string? createdBy,
        string? reference,
        Guid domesticPaymentConsentId) : base(id, created, createdBy, reference)
    {
        DomesticPaymentConsentId = domesticPaymentConsentId;
    }

    /// <summary>
    ///     Optional list of warning messages from Open Banking Connector.
    /// </summary>
    public IList<string>? Warnings { get; set; }

    public Guid DomesticPaymentConsentId { get; }
}

/// <summary>
///     Response to Create requests.
/// </summary>
public class
    DomesticPaymentConsentAuthContextCreateResponse : DomesticPaymentConsentAuthContextReadResponse
{
    internal DomesticPaymentConsentAuthContextCreateResponse(
        Guid id,
        DateTimeOffset created,
        string? createdBy,
        string? reference,
        Guid domesticPaymentConsentId,
        string authUrl) : base(id, created, createdBy, reference, domesticPaymentConsentId)
    {
        AuthUrl = authUrl;
    }

    public string AuthUrl { get; }
}
