// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives
{
    /// <summary>
    ///     Fluent interface methods for Create.
    /// </summary>
    /// <typeparam name="TPublicRequest"></typeparam>
    /// <typeparam name="TPublicResponse"></typeparam>
    public interface ICreate2Context<in TPublicRequest, TPublicResponse>
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
        Task<IFluentResponse<TPublicResponse>> CreateAsync(
            TPublicRequest request,
            Guid consentId,
            string? createdBy = null);
    }

    internal interface
        ICreate2ContextInternal<in TPublicRequest, TPublicResponse> :
            ICreate2Context<TPublicRequest, TPublicResponse>, IBaseContextInternal
        where TPublicRequest : class, ISupportsValidation
        where TPublicResponse : class
    {
        IObjectCreate2<TPublicRequest, TPublicResponse> CreateObject { get; }

        async Task<IFluentResponse<TPublicResponse>> ICreate2Context<TPublicRequest, TPublicResponse>.CreateAsync(
            TPublicRequest request,
            Guid consentId,
            string? createdBy)
        {
            request.ArgNotNull(nameof(request));

            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Validate request data and convert to messages
            ValidationResult validationResult = await request.ValidateAsync();
            IEnumerable<IFluentResponseInfoOrWarningMessage> newNonErrorMessages =
                validationResult.ProcessValidationResultsAndReturnBadRequestErrorMessages(
                    "prefix",
                    out
                    IList<FluentResponseBadRequestErrorMessage> badRequestErrorMessages);
            nonErrorMessages.AddRange(newNonErrorMessages);

            // If any request errors, terminate execution
            if (badRequestErrorMessages.Any())
            {
                IEnumerable<IFluentBadRequestErrorResponseMessage>
                    badRequestErrorMessagesAEnumerable =
                        badRequestErrorMessages; // use IEnumerable<T> for covariant cast
                IEnumerable<IFluentBadRequestErrorResponseMessage> messages =
                    badRequestErrorMessagesAEnumerable.Concat(nonErrorMessages);
                return new FluentBadRequestErrorResponse<TPublicResponse>(
                    messages: messages.ToList()); // ToList() is workaround for IEnumerable to IReadOnlyList conversion
            }

            // Execute operation catching errors 
            try
            {
                (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
                    await CreateObject.CreateAsync(
                        request,
                        consentId,
                        createdBy);
                nonErrorMessages.AddRange(postEntityNonErrorMessages);

                // Return success response (thrown exceptions produce error response)
                return new FluentSuccessResponse<TPublicResponse>(
                    response,
                    nonErrorMessages);
            }
            catch (AggregateException ex)
            {
                Context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse<TPublicResponse>(
                    messages: ex.CreateOtherErrorMessages()
                        .ToList()); // ToList() is workaround for IList to IReadOnlyList conversion; see https://github.com/dotnet/runtime/issues/31001
            }
            catch (Exception ex)
            {
                Context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse<TPublicResponse>(
                    new List<FluentResponseOtherErrorMessage> { ex.CreateOtherErrorMessage() });
            }
        }
    }
}
