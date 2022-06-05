// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FluentValidation;
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
        Task<TPublicPostResponse> CreateLocalAsync(
            TPublicRequest publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null);
    }

    internal interface
        ICreateLocalContextInternal<in TPublicRequest, TPublicResponse> :
            ICreateLocalContext<TPublicRequest, TPublicResponse>
        where TPublicRequest : class, ISupportsValidation
        where TPublicResponse : class
    {
        IObjectCreate<TPublicRequest, TPublicResponse> CreateLocalObject { get; }

        async Task<TPublicResponse> ICreateLocalContext<TPublicRequest, TPublicResponse>.
            CreateLocalAsync(
                TPublicRequest publicRequest,
                string? createdBy,
                string? apiRequestWriteFile,
                string? apiResponseWriteFile,
                string? apiResponseOverrideFile)
        {
            publicRequest.ArgNotNull(nameof(publicRequest));

            // Validate request data and convert to messages
            ValidationResult validationResult = await publicRequest.ValidateAsync();
            if (validationResult.Errors.Any(failure => failure.Severity == Severity.Error))
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Execute operation catching errors 
            (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
                await CreateLocalObject.CreateAsync(
                    publicRequest,
                    createdBy,
                    apiRequestWriteFile,
                    apiResponseWriteFile,
                    apiResponseOverrideFile);

            return response;
        }
    }
}
