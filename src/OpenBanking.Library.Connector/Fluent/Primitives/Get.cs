// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using DomesticVrpPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrp;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;
using DomesticVrpConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.
    DomesticVrpConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives
{
    internal abstract class GetBase<TPublicQuery, TPublicResponse>
        where TPublicResponse : class
    {
        private readonly ISharedContext _context;
        private readonly IObjectGet<TPublicQuery, TPublicResponse> _getObject;

        internal GetBase(ISharedContext context, IObjectGet<TPublicQuery, TPublicResponse> getObject)
        {
            _context = context;
            _getObject = getObject;
        }

        public async Task<IFluentResponse<TPublicResponse>> GetAsync(
            Guid id,
            string? modifiedBy,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            try
            {
                (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
                    await _getObject.GetAsync(
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

        public async Task<IFluentResponse<IQueryable<TPublicResponse>>> GetAsync(
            Expression<Func<TPublicQuery, bool>> predicate)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            try
            {
                (IQueryable<TPublicResponse> response,
                        IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
                    await _getObject.GetAsync(predicate);
                nonErrorMessages.AddRange(postEntityNonErrorMessages);

                // Return success response (thrown exceptions produce error response)
                return new FluentSuccessResponse<IQueryable<TPublicResponse>>(
                    response,
                    nonErrorMessages);
            }
            catch (AggregateException ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse<IQueryable<TPublicResponse>>(
                    messages: ex.CreateOtherErrorMessages()
                        .ToList()); // ToList() is workaround for IList to IReadOnlyList conversion; see https://github.com/dotnet/runtime/issues/31001
            }
            catch (Exception ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse<IQueryable<TPublicResponse>>(
                    new List<FluentResponseOtherErrorMessage> { ex.CreateOtherErrorMessage() });
            }
        }
    }

    internal class
        LocalEntityGet<TEntity, TPublicQuery, TPublicResponse> :
            GetBase<TPublicQuery, TPublicResponse>
        where TEntity : class, ISupportsFluentLocalEntityGet<TPublicResponse>, IEntity, new()
        where TPublicResponse : class
    {
        internal LocalEntityGet(ISharedContext context) : base(
            context,
            new Operations.LocalEntityGet<TEntity, TPublicQuery, TPublicResponse>(
                context.DbService.GetDbEntityMethodsClass<TEntity>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.TimeProvider,
                context.SoftwareStatementProfileCachedRepo,
                context.Instrumentation)) { }
    }

    internal class DomesticPaymentConsentsGet :
        GetBase<IDomesticPaymentConsentPublicQuery, DomesticPaymentConsentResponse>
    {
        internal DomesticPaymentConsentsGet(ISharedContext context) : base(
            context,
            new DomesticPaymentConsentGet(
                context.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.TimeProvider,
                context.SoftwareStatementProfileCachedRepo,
                context.Instrumentation,
                context.ApiVariantMapper)) { }
    }

    internal class DomesticPaymentConsentFundsConfirmationGet :
        GetBase<IDomesticPaymentConsentPublicQuery, DomesticPaymentConsentResponse>
    {
        internal DomesticPaymentConsentFundsConfirmationGet(ISharedContext context) : base(
            context,
            new DomesticPaymentConsentGetFundsConfirmation(
                context.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.TimeProvider,
                context.SoftwareStatementProfileCachedRepo,
                context.Instrumentation,
                context.ApiVariantMapper)) { }
    }

    internal class DomesticVrpConsentFundsConfirmationGet :
        GetBase<IDomesticVrpConsentPublicQuery, DomesticVrpConsentResponse>
    {
        internal DomesticVrpConsentFundsConfirmationGet(ISharedContext context) : base(
            context,
            new DomesticVrpConsentGetFundsConfirmation(
                context.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.TimeProvider,
                context.SoftwareStatementProfileCachedRepo,
                context.Instrumentation,
                context.ApiVariantMapper)) { }
    }

    internal class DomesticPaymentsGet :
        GetBase<IDomesticPaymentPublicQuery, DomesticPaymentResponse>
    {
        internal DomesticPaymentsGet(ISharedContext context) : base(
            context,
            new DomesticPaymentGet(
                context.DbService.GetDbEntityMethodsClass<DomesticPayment>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.TimeProvider,
                context.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                context.SoftwareStatementProfileCachedRepo,
                context.Instrumentation,
                context.ApiVariantMapper)) { }
    }

    internal class DomesticVrpConsentGet :
        GetBase<IDomesticVrpConsentPublicQuery, DomesticVrpConsentResponse>
    {
        internal DomesticVrpConsentGet(ISharedContext context) : base(
            context,
            new Operations.VariableRecurringPayments.DomesticVrpConsentGet(
                context.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.TimeProvider,
                context.SoftwareStatementProfileCachedRepo,
                context.Instrumentation,
                context.ApiVariantMapper)) { }
    }

    internal class DomesticVrpGet :
        GetBase<IDomesticVrpPublicQuery, DomesticVrpResponse>
    {
        internal DomesticVrpGet(ISharedContext context) : base(
            context,
            new Operations.VariableRecurringPayments.DomesticVrpGet(
                context.DbService.GetDbEntityMethodsClass<DomesticVrpPersisted>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.TimeProvider,
                context.SoftwareStatementProfileCachedRepo,
                context.Instrumentation,
                context.ApiVariantMapper)) { }
    }
}
