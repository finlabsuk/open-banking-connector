// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;

public interface IDomesticVrpConsentPublicQuery : IBaseQuery
{
    /// <summary>
    ///     Associated BankRegistration object
    /// </summary>
    public Guid BankRegistrationId { get; }

    /// <summary>
    ///     Associated VariableRecurringPaymentsApi object
    /// </summary>
    public Guid VariableRecurringPaymentsApiId { get; }

    /// <summary>
    ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
    ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
    /// </summary>
    public string ExternalApiId { get; }
}

public abstract class DomesticVrpConsentBaseResponse : ConsentResponseBase, IDomesticVrpConsentPublicQuery
{
    internal DomesticVrpConsentBaseResponse(
        Guid id,
        DateTimeOffset created,
        string? createdBy,
        string? reference,
        IList<string>? warnings,
        Guid bankRegistrationId,
        string externalApiId,
        string? externalApiUserId,
        DateTimeOffset authContextModified,
        string? authContextModifiedBy,
        Guid variableRecurringPaymentsApiId) : base(
        id,
        created,
        createdBy,
        reference,
        warnings,
        bankRegistrationId,
        externalApiId,
        externalApiUserId,
        authContextModified,
        authContextModifiedBy)
    {
        VariableRecurringPaymentsApiId = variableRecurringPaymentsApiId;
    }

    /// <summary>
    ///     Associated VariableRecurringPaymentsApi object
    /// </summary>
    public Guid VariableRecurringPaymentsApiId { get; }
}

/// <summary>
///     Response to DomesticVrpConsent Create and Read requests.
/// </summary>
public class DomesticVrpConsentCreateResponse : DomesticVrpConsentBaseResponse
{
    internal DomesticVrpConsentCreateResponse(
        Guid id,
        DateTimeOffset created,
        string? createdBy,
        string? reference,
        IList<string>? warnings,
        Guid bankRegistrationId,
        string externalApiId,
        string? externalApiUserId,
        DateTimeOffset authContextModified,
        string? authContextModifiedBy,
        Guid variableRecurringPaymentsApiId,
        VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse? externalApiResponse) : base(
        id,
        created,
        createdBy,
        reference,
        warnings,
        bankRegistrationId,
        externalApiId,
        externalApiUserId,
        authContextModified,
        authContextModifiedBy,
        variableRecurringPaymentsApiId)
    {
        ExternalApiResponse = externalApiResponse;
    }

    public VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse? ExternalApiResponse { get; }
}

/// <summary>
///     Response to DomesticVrpConsent ReadFundsConfirmation requests.
/// </summary>
public class DomesticVrpConsentReadFundsConfirmationResponse : DomesticVrpConsentBaseResponse

{
    internal DomesticVrpConsentReadFundsConfirmationResponse(
        Guid id,
        DateTimeOffset created,
        string? createdBy,
        string? reference,
        IList<string>? warnings,
        Guid bankRegistrationId,
        string externalApiId,
        string? externalApiUserId,
        DateTimeOffset authContextModified,
        string? authContextModifiedBy,
        Guid variableRecurringPaymentsApiId,
        VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse externalApiResponse) : base(
        id,
        created,
        createdBy,
        reference,
        warnings,
        bankRegistrationId,
        externalApiId,
        externalApiUserId,
        authContextModified,
        authContextModifiedBy,
        variableRecurringPaymentsApiId)
    {
        ExternalApiResponse = externalApiResponse;
    }

    public VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse ExternalApiResponse { get; }
}
