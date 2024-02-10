// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FluentValidation;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

/// <summary>
///     Fluent interface methods for Read.
/// </summary>
/// <typeparam name="TPublicResponse"></typeparam>
/// <typeparam name="TPublicRequest"></typeparam>
public interface ICreateVrpConsentFundsConfirmationContext<in TPublicRequest, TPublicResponse>
    where TPublicResponse : class
{
    /// <summary>
    ///     CREATE funds confirmation for consent (includes POSTing object to bank API).
    /// </summary>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <param name="publicRequestUrlWithoutQuery"></param>
    /// <returns></returns>
    Task<TPublicResponse> CreateFundsConfirmationAsync(
        TPublicRequest request,
        Guid id,
        string? publicRequestUrlWithoutQuery = null);
}

internal interface
    ICreateVrpConsentFundsConfirmationContextInternal<in TPublicRequest, TPublicResponse> :
    ICreateVrpConsentFundsConfirmationContext<TPublicRequest, TPublicResponse>
    where TPublicRequest : class, ISupportsValidation
    where TPublicResponse : class
{
    IVrpConsentFundsConfirmationCreate<TPublicRequest, TPublicResponse> CreateVrpConsentFundsConfirmation { get; }

    async Task<TPublicResponse> ICreateVrpConsentFundsConfirmationContext<TPublicRequest, TPublicResponse>.
        CreateFundsConfirmationAsync(
            TPublicRequest request,
            Guid id,
            string? publicRequestUrlWithoutQuery)
    {
        request.ArgNotNull(nameof(request));

        // Validate request data and convert to messages
        ValidationResult validationResult = await request.ValidateAsync();
        if (validationResult.Errors.Any(failure => failure.Severity == Severity.Error))
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Execute operation catching errors
        var vrpConsentFundsConfirmationCreateParams =
            new VrpConsentFundsConfirmationCreateParams
            {
                PublicRequestUrlWithoutQuery = publicRequestUrlWithoutQuery,
                Id = id
            };
        (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
            await CreateVrpConsentFundsConfirmation.CreateFundsConfirmationAsync(
                request,
                vrpConsentFundsConfirmationCreateParams);

        return response;
    }
}
