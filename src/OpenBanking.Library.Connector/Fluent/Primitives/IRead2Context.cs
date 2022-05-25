// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives
{
    /// <summary>
    ///     Fluent interface methods for Read.
    /// </summary>
    /// <typeparam name="TPublicResponse"></typeparam>
    public interface IRead2Context<TPublicResponse>
        where TPublicResponse : class
    {
        /// <summary>
        ///     READ objects using consent ID (includes GETing objects from bank API).
        ///     Objects will be read from bank database only.
        /// </summary>
        /// <param name="consentId"></param>
        /// <param name="externalApiAccountId"></param>
        /// <param name="externalApiStatementId"></param>
        /// <param name="fromBookingDateTime"></param>
        /// <param name="toBookingDateTime"></param>
        /// <param name="page"></param>
        /// <param name="modifiedBy"></param>
        /// <param name="requestUrlWithoutQuery"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        Task<IFluentResponse<TPublicResponse>> ReadAsync(
            Guid consentId,
            string? externalApiAccountId = null,
            string? externalApiStatementId = null,
            string? fromBookingDateTime = null,
            string? toBookingDateTime = null,
            string? page = null,
            string? modifiedBy = null,
            string? requestUrlWithoutQuery = null,
            string? queryString = null);
    }

    internal interface
        IRead2ContextInternal<TPublicResponse> : IRead2Context<TPublicResponse>,
            IBaseContextInternal
        where TPublicResponse : class
    {
        IObjectRead2<TPublicResponse> ReadObject { get; }

        async Task<IFluentResponse<TPublicResponse>> IRead2Context<TPublicResponse>.ReadAsync(
            Guid consentId,
            string? externalApiAccountId,
            string? externalApiStatementId,
            string? fromBookingDateTime,
            string? toBookingDateTime,
            string? page,
            string? modifiedBy,
            string? requestUrlWithoutQuery,
            string? queryString)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            try
            {
                (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
                    await ReadObject.ReadAsync(
                        consentId,
                        externalApiAccountId,
                        externalApiStatementId,
                        fromBookingDateTime,
                        toBookingDateTime,
                        page,
                        modifiedBy,
                        requestUrlWithoutQuery,
                        queryString);
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
