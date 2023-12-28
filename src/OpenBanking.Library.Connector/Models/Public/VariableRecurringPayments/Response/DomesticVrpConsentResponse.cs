// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;

public interface IDomesticVrpConsentPublicQuery : IEntityBaseQuery
{
    /// <summary>
    ///     Associated BankRegistration object
    /// </summary>
    public Guid BankRegistrationId { get; }

    /// <summary>
    ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
    ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
    /// </summary>
    public string ExternalApiId { get; }
}

public abstract class DomesticVrpConsentBaseResponse : ConsentBaseResponse, IDomesticVrpConsentPublicQuery { }

/// <summary>
///     Response to DomesticVrpConsent Create and Read requests.
/// </summary>
public class DomesticVrpConsentCreateResponse : DomesticVrpConsentBaseResponse
{
    public VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse? ExternalApiResponse { get; init; }
}

/// <summary>
///     Response to DomesticVrpConsent ReadFundsConfirmation requests.
/// </summary>
public class DomesticVrpConsentReadFundsConfirmationResponse : DomesticVrpConsentBaseResponse

{
    public required VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse ExternalApiResponse
    {
        get;
        init;
    }
}
