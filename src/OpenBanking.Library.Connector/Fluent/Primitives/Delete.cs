// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives
{
    internal abstract class DeleteBase
    {
        private readonly ISharedContext _context;
        private readonly IObjectDelete _deleteObject;

        internal DeleteBase(ISharedContext context, IObjectDelete deleteObject)
        {
            _context = context;
            _deleteObject = deleteObject;
        }

        public async Task<IFluentResponse> DeleteAsync(
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
                    await _deleteObject.DeleteAsync(
                        id,
                        modifiedBy,
                        useRegistrationAccessToken);
                nonErrorMessages.AddRange(postEntityNonErrorMessages);

                // Return success response (thrown exceptions produce error response)
                return new FluentSuccessResponse(nonErrorMessages);
            }
            catch (AggregateException ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse(
                    messages: ex.CreateOtherErrorMessages()
                        .ToList()); // ToList() is workaround for IList to IReadOnlyList conversion; see https://github.com/dotnet/runtime/issues/31001
            }
            catch (Exception ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse(
                    new List<FluentResponseOtherErrorMessage> { ex.CreateOtherErrorMessage() });
            }
        }
    }

    internal class LocalEntityDelete<TEntity> : DeleteBase
        where TEntity : class, ISupportsFluentDeleteLocal<TEntity>
    {
        internal LocalEntityDelete(ISharedContext context) : base(
            context,
            new Operations.LocalEntityDelete<TEntity>(
                context.DbService.GetDbEntityMethodsClass<TEntity>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.TimeProvider,
                context.SoftwareStatementProfileCachedRepo)) { }
    }

    internal class BankRegistrationDelete : DeleteBase
    {
        internal BankRegistrationDelete(ISharedContext context) : base(
            context,
            new Operations.BankRegistrationDelete(
                context.DbService.GetDbEntityMethodsClass<BankRegistration>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.TimeProvider,
                context.SoftwareStatementProfileCachedRepo)) { }
    }
}
