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
    ///     Fluent interface methods for CreateLocal.
    /// </summary>
    /// <typeparam name="TPublicRequest"></typeparam>
    /// <typeparam name="TPublicPostResponse"></typeparam>
    public interface ICreateLocalContext<in TPublicRequest, TPublicPostResponse>
        where TPublicPostResponse : class
    {
        /// <summary>
        ///     CREATE local object (does not include POSTing object to bank API).
        ///     Object will be created in local database only.
        /// </summary>
        /// <param name="publicRequest">Request object</param>
        /// <param name="createdBy">Optional user name or comment for DB record(s).</param>
        /// <param name="apiRequestWriteFile"></param>
        /// <param name="apiResponseWriteFile"></param>
        /// <param name="apiResponseOverrideFile"></param>
        /// <returns></returns>
        Task<IFluentResponse<TPublicPostResponse>> CreateLocalAsync(
            TPublicRequest publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null);
    }

    internal interface
        ICreateLocalContextInternal<in TPublicRequest, TPublicResponse> :
            ICreateLocalContext<TPublicRequest, TPublicResponse>, IBaseContextInternal
        where TPublicRequest : class, ISupportsValidation
        where TPublicResponse : class
    {
        IObjectCreate<TPublicRequest, TPublicResponse> CreateLocalObject { get; }

        async Task<IFluentResponse<TPublicResponse>> ICreateLocalContext<TPublicRequest, TPublicResponse>.
            CreateLocalAsync(
                TPublicRequest publicRequest,
                string? createdBy,
                string? apiRequestWriteFile,
                string? apiResponseWriteFile,
                string? apiResponseOverrideFile)
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
                (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
                    await CreateLocalObject.CreateAsync(
                        publicRequest,
                        createdBy,
                        apiRequestWriteFile,
                        apiResponseWriteFile,
                        apiResponseOverrideFile);
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
