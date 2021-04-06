// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.ApiModels.Base;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    /// <summary>
    ///     Fluent interface for POST-only data objects. These are similar to entities except they are not persisted to DB
    ///     either locally or at bank API and
    ///     only a POST method is available.
    ///     The main current use case is passing authorisation redirect objects which relate to a consent of unknown type and
    ///     ID.
    ///     OBC will try to get a token from bank API and update the appropriate consent if the authorisation has been
    ///     successful.
    /// </summary>
    /// <typeparam name="TPublicRequest"></typeparam>
    /// <typeparam name="TPublicResponse"></typeparam>
    public interface IPostContext<in TPublicRequest, TPublicResponse>
        where TPublicResponse : class
    {
        /// <summary>
        ///     POST entity to Open Banking Connector.
        /// </summary>
        /// <param name="publicRequest">Entity request object.</param>
        /// <param name="modifiedBy">Optional user name or comment for DB record(s).</param>
        /// <returns></returns>
        Task<IFluentResponse<TPublicResponse>> PostAsync(TPublicRequest publicRequest, string? createdBy = null);
    }

    internal class
        PostContext<TPostOnly, TPublicRequest, TPublicResponse>
        : IPostContext<TPublicRequest, TPublicResponse>
        where TPostOnly : class, ISupportsFluentPost<TPublicRequest, TPublicResponse>, new()
        where TPublicRequest : class, ISupportsValidation
        where TPublicResponse : class
    {
        private readonly ISharedContext _context;

        internal PostContext(ISharedContext context)
        {
            _context = context;
        }

        public async Task<IFluentResponse<TPublicResponse>> PostAsync(TPublicRequest publicRequest, string? createdBy)
        {
            publicRequest.ArgNotNull(nameof(publicRequest));

            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Validate request data and convert to messages
            ValidationResult validationResult = await publicRequest.ValidateAsync();
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
                var (response, postEntityNonErrorMessages) =
                    await new TPostOnly().PostEntityAsyncWrapper(
                        _context,
                        publicRequest,
                        createdBy);
                nonErrorMessages.AddRange(postEntityNonErrorMessages);

                // Return success response (thrown exceptions produce error response)
                return new FluentSuccessResponse<TPublicResponse>(
                    response,
                    nonErrorMessages);
            }
            catch (AggregateException ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse<TPublicResponse>(
                    messages: ex.CreateOtherErrorMessages()
                        .ToList()); // ToList() is workaround for IList to IReadOnlyList conversion; see https://github.com/dotnet/runtime/issues/31001
            }
            catch (Exception ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse<TPublicResponse>(
                    new List<FluentResponseOtherErrorMessage> { ex.CreateOtherErrorMessage() });
            }
        }
    }
}
