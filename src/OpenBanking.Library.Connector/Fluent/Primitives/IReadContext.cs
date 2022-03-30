// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives
{
    /// <summary>
    ///     Fluent interface methods for Read.
    /// </summary>
    /// <typeparam name="TPublicResponse"></typeparam>
    public interface IReadContext<TPublicResponse>
        where TPublicResponse : class
    {
        /// <summary>
        ///     READ object by ID (includes GETing object from bank API).
        ///     Object will be read from bank and also from local database if it is a Bank Registration or Consent.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifiedBy"></param>
        /// <param name="apiResponseWriteFile"></param>
        /// <param name="apiResponseOverrideFile"></param>
        /// <returns></returns>
        Task<IFluentResponse<TPublicResponse>> ReadAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null);
    }

    internal interface
        IReadContextInternal<TPublicResponse> : IReadContext<TPublicResponse>,
            IBaseContextInternal
        where TPublicResponse : class
    {
        IObjectRead<TPublicResponse> ReadObject { get; }

        async Task<IFluentResponse<TPublicResponse>> IReadContext<TPublicResponse>.ReadAsync(
            Guid id,
            string? modifiedBy,
            string? apiResponseWriteFile,
            string? apiResponseOverrideFile)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            try
            {
                (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
                    await ReadObject.ReadAsync(
                        id,
                        modifiedBy,
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
