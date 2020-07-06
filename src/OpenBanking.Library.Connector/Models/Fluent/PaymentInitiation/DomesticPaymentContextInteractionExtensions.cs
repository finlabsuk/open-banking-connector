// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation.PaymentInitialisation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation
{
    public static class DomesticPaymentContextInteractionExtensions
    {
        public static DomesticPaymentContext Data(this DomesticPaymentContext context, DomesticPaymentRequest value)
        {
            context.Data = value;
            return context;
        }

        public static DomesticPaymentContext ConsentId(this DomesticPaymentContext context, string value)
        {
            context.ConsentId = value;
            return context;
        }

        public static async Task<DomesticPaymentFluentResponse> SubmitAsync(this DomesticPaymentContext context)
        {
            context.ArgNotNull(nameof(context));
            try
            {
                DomesticPaymentRequest domesticPayment = context.Data ?? new DomesticPaymentRequest(
                    context.ConsentId.ArgNotNullElseInvalidOp("ConsentId not specified"));

                IList<FluentResponseMessage> validationErrors = new PaymentRequestValidator()
                    .Validate(domesticPayment)
                    .GetOpenBankingResponses();
                if (validationErrors.Count > 0)
                {
                    return new DomesticPaymentFluentResponse(validationErrors);
                }

                CreateDomesticPayment i = new CreateDomesticPayment(
                    apiClient: context.Context.ApiClient,
                    mapper: context.Context.EntityMapper,
                    openBankingClientRepo: context.Context.ClientProfileRepository,
                    domesticConsentRepo: context.Context.DomesticConsentRepository,
                    apiProfileRepo: context.Context.ApiProfileRepository,
                    softwareStatementProfileService: context.Context.SoftwareStatementProfileService);

                OBWriteDomesticResponse resp = await i.CreateAsync(domesticPayment.ConsentId);

                return new DomesticPaymentFluentResponse(resp.Data);
            }
            catch (AggregateException ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new DomesticPaymentFluentResponse(ex.CreateErrorMessages());
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new DomesticPaymentFluentResponse(ex.CreateErrorMessage());
            }
        }
    }
}
