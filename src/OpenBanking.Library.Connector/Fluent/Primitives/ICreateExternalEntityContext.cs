// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FluentValidation;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

/// <summary>
///     Fluent interface methods for Create.
/// </summary>
/// <typeparam name="TPublicRequest"></typeparam>
/// <typeparam name="TPublicResponse"></typeparam>
public interface ICreateExternalEntityContext<in TPublicRequest, TPublicResponse>
    where TPublicResponse : class
{
    /// <summary>
    ///     CREATE object (includes POSTing object to bank API).
    ///     Object will be created at bank and also in local database if it is a Bank Registration or Consent.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="consentId"></param>
    /// <param name="createdBy"></param>
    /// <returns></returns>
    Task<TPublicResponse> CreateAsync(
        TPublicRequest request,
        Guid consentId,
        string? createdBy = null);
}

internal interface
    ICreateExternalEntityContextInternal<in TPublicRequest, TPublicResponse> :
        ICreateExternalEntityContext<TPublicRequest, TPublicResponse>
    where TPublicRequest : class, ISupportsValidation
    where TPublicResponse : class
{
    IExternalCreate<TPublicRequest, TPublicResponse> CreateObject { get; }

    async Task<TPublicResponse> ICreateExternalEntityContext<TPublicRequest, TPublicResponse>.CreateAsync(
        TPublicRequest request,
        Guid consentId,
        string? createdBy)
    {
        request.ArgNotNull(nameof(request));

        // Validate request data and convert to messages
        ValidationResult validationResult = await request.ValidateAsync();
        if (validationResult.Errors.Any(failure => failure.Severity == Severity.Error))
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Execute operation catching errors 
        (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
            await CreateObject.CreateAsync(
                request,
                consentId,
                createdBy);

        return response;
    }
}
