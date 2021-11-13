// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FluentValidation.Results;
using BankPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Bank;
using BankApiSetPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankApiSet;
using BankRegistrationPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankRegistration;
using DomesticVrpPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrp;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;
using DomesticVrpConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.
    DomesticVrpConsentAuthContext;
using DomesticVrpConsentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrpConsent;
using DomesticPaymentConsentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPaymentConsent;
using DomesticPaymentConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.
    DomesticPaymentConsentAuthContext;
using DomesticVrpConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.
    DomesticVrpConsentAuthContext;
using DomesticPaymentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPayment;
using DomesticVrpRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrp;
using BankRegistrationRequest = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.BankRegistration;
using BaseRequest = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.Base;
using DomesticPaymentConsentAuthContext =
    FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation.DomesticPaymentConsentAuthContext;
using DomesticPaymentConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives
{
    internal abstract class PostBase<TPublicRequest, TPublicResponse>
        where TPublicRequest : class, ISupportsValidation
        where TPublicResponse : class
    {
        private readonly ISharedContext _context;
        private readonly IObjectPost<TPublicRequest, TPublicResponse> _postObject;

        internal PostBase(ISharedContext context, IObjectPost<TPublicRequest, TPublicResponse> postObject)
        {
            _context = context;
            _postObject = postObject;
        }

        public async Task<IFluentResponse<TPublicResponse>> PostAsync(
            TPublicRequest publicRequest,
            string? createdBy,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null)
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
                    await _postObject.PostAsync(
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

    internal class LocalEntityPost<TEntity, TPublicRequest, TPublicResponse> :
        PostBase<TPublicRequest, TPublicResponse>
        where TEntity : class, IEntity, ISupportsFluentLocalEntityPost<TPublicRequest, TPublicResponse>,
        new()
        where TPublicRequest : BaseRequest, ISupportsValidation
        where TPublicResponse : class
    {
        internal LocalEntityPost(ISharedContext context) : base(
            context,
            new Operations.LocalEntityPost<TEntity, TPublicRequest, TPublicResponse>(
                context.DbService.GetDbEntityMethodsClass<TEntity>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.TimeProvider,
                context.SoftwareStatementProfileCachedRepo,
                context.Instrumentation)) { }
    }

    internal class DomesticPaymentConsentAuthPost :
        PostBase<DomesticPaymentConsentAuthContextRequest,
            DomesticPaymentConsentAuthContextPostResponse>
    {
        internal DomesticPaymentConsentAuthPost(ISharedContext context) : base(
            context,
            new DomesticPaymentConsentAuthContext(
                context.DbService.GetDbEntityMethodsClass<DomesticPaymentConsentAuthContextPersisted>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.TimeProvider,
                context.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                context.SoftwareStatementProfileCachedRepo,
                context.Instrumentation)) { }
    }

    internal class DomesticPaymentConsentsPost :
        PostBase<DomesticPaymentConsentRequest, DomesticPaymentConsentResponse>
    {
        internal DomesticPaymentConsentsPost(ISharedContext context) : base(
            context,
            new DomesticPaymentConsentPost(
                context.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.TimeProvider,
                context.SoftwareStatementProfileCachedRepo,
                context.Instrumentation,
                context.ApiVariantMapper,
                context.DbService.GetDbEntityMethodsClass<BankApiSetPersisted>(),
                context.DbService.GetDbEntityMethodsClass<BankRegistrationPersisted>())) { }
    }

    internal class DomesticPaymentsPost :
        PostBase<DomesticPaymentRequest, DomesticPaymentResponse>
    {
        internal DomesticPaymentsPost(ISharedContext context) : base(
            context,
            new DomesticPaymentPost(
                context.DbService.GetDbEntityMethodsClass<DomesticPayment>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.TimeProvider,
                context.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                context.SoftwareStatementProfileCachedRepo,
                context.Instrumentation,
                context.ApiVariantMapper)) { }
    }

    internal class DomesticVrpConsentPost :
        PostBase<DomesticVrpConsentRequest, DomesticVrpConsentResponse>
    {
        internal DomesticVrpConsentPost(ISharedContext context) : base(
            context,
            new Operations.VariableRecurringPayments.DomesticVrpConsentPost(
                context.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.TimeProvider,
                context.SoftwareStatementProfileCachedRepo,
                context.Instrumentation,
                context.ApiVariantMapper,
                context.DbService.GetDbEntityMethodsClass<BankApiSetPersisted>(),
                context.DbService.GetDbEntityMethodsClass<BankRegistrationPersisted>())) { }
    }

    internal class DomesticVrpConsentAuthPost :
        PostBase<DomesticVrpConsentAuthContextRequest,
            DomesticVrpConsentAuthContextPostResponse>
    {
        internal DomesticVrpConsentAuthPost(ISharedContext context) : base(
            context,
            new DomesticVrpConsentAuthContext(
                context.DbService.GetDbEntityMethodsClass<DomesticVrpConsentAuthContextPersisted>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.TimeProvider,
                context.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                context.SoftwareStatementProfileCachedRepo,
                context.Instrumentation)) { }
    }

    internal class DomesticVrpPost :
        PostBase<DomesticVrpRequest, DomesticVrpResponse>
    {
        internal DomesticVrpPost(ISharedContext context) : base(
            context,
            new Operations.VariableRecurringPayments.DomesticVrpPost(
                context.DbService.GetDbEntityMethodsClass<DomesticVrpPersisted>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.TimeProvider,
                context.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                context.SoftwareStatementProfileCachedRepo,
                context.Instrumentation,
                context.ApiVariantMapper)) { }
    }

    internal class BankRegistrationsPost :
        PostBase<BankRegistrationRequest, BankRegistrationResponse>
    {
        internal BankRegistrationsPost(ISharedContext sharedContext) : base(
            sharedContext,
            new BankRegistrationPost(
                sharedContext.DbService.GetDbEntityMethodsClass<BankRegistrationPersisted>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation,
                sharedContext.ApiVariantMapper,
                sharedContext.ApiClient,
                sharedContext.DbService.GetDbEntityMethodsClass<BankPersisted>())) { }
    }

    internal class AuthAuthResultsPost :
        PostBase<AuthResult, DomesticPaymentConsentAuthContextResponse>
    {
        internal AuthAuthResultsPost(ISharedContext sharedContext) : base(
            sharedContext,
            new AuthContextAuthResult(
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.DbService.GetDbEntityMethodsClass<DomesticPaymentConsentAuthContextPersisted>(),
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation)) { }
    }
}
