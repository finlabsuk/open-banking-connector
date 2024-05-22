// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FluentValidation;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

/// <summary>
///     Fluent interface methods for Create.
/// </summary>
/// <typeparam name="TPublicRequest"></typeparam>
/// <typeparam name="TPublicResponse"></typeparam>
public interface ICreateConsentContext<in TPublicRequest, TPublicResponse>
    where TPublicRequest : class, ISupportsValidation
    where TPublicResponse : class
{
    private protected IObjectCreate<TPublicRequest, TPublicResponse, ConsentCreateParams> CreateObject { get; }

    /// <summary>
    ///     CREATE object (includes POSTing object to bank API).
    ///     Object will be created at bank and also in local database if it is a Bank Registration or Consent.
    /// </summary>
    /// <param name="publicRequest">Request object</param>
    /// <param name="publicRequestUrlWithoutQuery"></param>
    /// <param name="extraHeaders"></param>
    /// <returns></returns>
    async Task<TPublicResponse> CreateAsync(
        TPublicRequest publicRequest,
        string? publicRequestUrlWithoutQuery = null,
        IEnumerable<HttpHeader>? extraHeaders = null)
    {
        publicRequest.ArgNotNull(nameof(publicRequest));

        // Validate request data and convert to messages
        ValidationResult validationResult = await publicRequest.ValidateAsync();
        if (validationResult.Errors.Any(failure => failure.Severity == Severity.Error))
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Execute operation catching errors
        var consentCreateParams =
            new ConsentCreateParams
            {
                PublicRequestUrlWithoutQuery = publicRequestUrlWithoutQuery,
                ExtraHeaders = extraHeaders
            };
        (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
            await CreateObject.CreateAsync(publicRequest, consentCreateParams);

        return response;
    }
}
