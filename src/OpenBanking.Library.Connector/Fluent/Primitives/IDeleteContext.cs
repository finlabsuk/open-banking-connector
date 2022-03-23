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
    ///     Fluent interface methods for Delete.
    /// </summary>
    public interface IDeleteContext
    {
        /// <summary>
        ///     DELETE object by ID (includes DELETE-ing object at bank API).
        ///     Object will be deleted at bank and also from local database if it is a Bank Registration or Consent.
        ///     Note: deletions from local database are implemented via soft delete (i.e. a flag is set).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifiedBy">Optional user name or comment for local DB update when performing soft delete.</param>
        /// <param name="useRegistrationAccessToken"></param>
        /// <returns></returns>
        Task<IFluentResponse> DeleteAsync(
            Guid id,
            string? modifiedBy = null,
            bool useRegistrationAccessToken = false);
    }

    internal interface IDeleteContextInternal : IDeleteContext, IBaseContextInternal
    {
        IObjectDelete DeleteObject { get; }

        async Task<IFluentResponse> IDeleteContext.DeleteAsync(
            Guid id,
            string? modifiedBy,
            bool useRegistrationAccessToken)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            try
            {
                IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages =
                    await DeleteObject.DeleteAsync(
                        id,
                        modifiedBy,
                        useRegistrationAccessToken);
                nonErrorMessages.AddRange(postEntityNonErrorMessages);

                // Return success response (thrown exceptions produce error response)
                return new FluentSuccessResponse(nonErrorMessages);
            }
            catch (AggregateException ex)
            {
                Context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse(
                    messages: ex.CreateOtherErrorMessages()
                        .ToList()); // ToList() is workaround for IList to IReadOnlyList conversion; see https://github.com/dotnet/runtime/issues/31001
            }
            catch (Exception ex)
            {
                Context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse(
                    new List<FluentResponseOtherErrorMessage> { ex.CreateOtherErrorMessage() });
            }
        }
    }
}
