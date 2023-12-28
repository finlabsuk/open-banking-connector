// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;

public interface IDomesticPaymentConsentPublicQuery : IEntityBaseQuery
{
    /// <summary>
    ///     Associated BankRegistration object
    /// </summary>
    Guid BankRegistrationId { get; }

    /// <summary>
    ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
    ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
    /// </summary>
    string ExternalApiId { get; }
}

public abstract class DomesticPaymentConsentBaseResponse : ConsentBaseResponse,
    IDomesticPaymentConsentPublicQuery { }

/// <summary>
///     Response to DomesticPaymentConsent Create and Read requests
/// </summary>
public class DomesticPaymentConsentCreateResponse : DomesticPaymentConsentBaseResponse
{
    public PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5? ExternalApiResponse { get; init; }
}

/// <summary>
///     Response to DomesticPaymentConsent ReadFundsConfirmation requests
/// </summary>
public class DomesticPaymentConsentReadFundsConfirmationResponse : DomesticPaymentConsentBaseResponse
{
    public required PaymentInitiationModelsPublic.OBWriteFundsConfirmationResponse1 ExternalApiResponse { get; init; }
}
