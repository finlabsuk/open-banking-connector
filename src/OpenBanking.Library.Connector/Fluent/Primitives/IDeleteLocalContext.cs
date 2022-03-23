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
    ///     Fluent interface methods for DeleteLocal.
    /// </summary>
    public interface IDeleteLocalContext
    {
        /// <summary>
        ///     DELETE local object by ID (does not include DELETE-ing object from bank API).
        ///     Object will be deleted from local database only.
        ///     Note: deletions from local database are implemented via soft delete (i.e. a flag is set).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifiedBy">Optional user name or comment for DB update when performing soft delete.</param>
        /// <returns></returns>
        Task<IFluentResponse> DeleteLocalAsync(
            Guid id,
            string? modifiedBy = null);
    }

    internal interface IDeleteLocalContextInternal : IDeleteLocalContext, IBaseContextInternal
    {
        IObjectDelete DeleteLocalObject { get; }

        async Task<IFluentResponse> IDeleteLocalContext.DeleteLocalAsync(
            Guid id,
            string? modifiedBy)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            try
            {
                IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages =
                    await DeleteLocalObject.DeleteAsync(
                        id,
                        modifiedBy,
                        false);
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
