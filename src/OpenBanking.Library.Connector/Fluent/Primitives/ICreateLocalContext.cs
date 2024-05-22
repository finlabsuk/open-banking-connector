// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FluentValidation;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

/// <summary>
///     Fluent interface methods for CreateLocal.
/// </summary>
/// <typeparam name="TPublicRequest"></typeparam>
/// <typeparam name="TPublicResponse"></typeparam>
public interface ICreateLocalContext<in TPublicRequest, TPublicResponse>
    where TPublicRequest : class, ISupportsValidation
    where TPublicResponse : class
{
    private protected IObjectCreate<TPublicRequest, TPublicResponse, LocalCreateParams> CreateLocalObject { get; }

    /// <summary>
    ///     CREATE local object (does not include POSTing object to bank API).
    ///     Object will be created in local database only.
    /// </summary>
    /// <param name="publicRequest">Request object</param>
    /// <returns></returns>
    async Task<TPublicResponse> CreateLocalAsync(TPublicRequest publicRequest)
    {
        publicRequest.ArgNotNull(nameof(publicRequest));

        // Validate request data and convert to messages
        ValidationResult validationResult = await publicRequest.ValidateAsync();
        if (validationResult.Errors.Any(failure => failure.Severity == Severity.Error))
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Execute operation catching errors
        var localCreateParams = new LocalCreateParams();
        (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
            await CreateLocalObject.CreateAsync(publicRequest, localCreateParams);

        return response;
    }
}
