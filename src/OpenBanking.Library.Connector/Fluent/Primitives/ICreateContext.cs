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
    ///     Fluent interface methods for Create.
    /// </summary>
    /// <typeparam name="TPublicRequest"></typeparam>
    /// <typeparam name="TPublicResponse"></typeparam>
    public interface ICreateContext<in TPublicRequest, TPublicResponse>
        where TPublicResponse : class
    {
        /// <summary>
        ///     CREATE object (includes POSTing object to bank API).
        ///     Object will be created at bank and also in local database if it is a Bank Registration or Consent.
        /// </summary>
        /// <param name="publicRequest">Request object</param>
        /// <param name="createdBy">Optional user name or comment for DB record(s).</param>
        /// <param name="apiRequestWriteFile"></param>
        /// <param name="apiResponseWriteFile"></param>
        /// <param name="apiResponseOverrideFile"></param>
        /// <returns></returns>
        Task<TPublicResponse> CreateAsync(
            TPublicRequest publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null);
    }

    internal interface
        ICreateContextInternal<in TPublicRequest, TPublicResponse> :
            ICreateContext<TPublicRequest, TPublicResponse>, IBaseContextInternal
        where TPublicRequest : class, ISupportsValidation
        where TPublicResponse : class
    {
        IObjectCreate<TPublicRequest, TPublicResponse> CreateObject { get; }

        async Task<TPublicResponse> ICreateContext<TPublicRequest, TPublicResponse>.CreateAsync(
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
                await CreateObject.CreateAsync(
                    publicRequest,
                    createdBy,
                    apiRequestWriteFile,
                    apiResponseWriteFile,
                    apiResponseOverrideFile);

            return response;
        }
    }
}
