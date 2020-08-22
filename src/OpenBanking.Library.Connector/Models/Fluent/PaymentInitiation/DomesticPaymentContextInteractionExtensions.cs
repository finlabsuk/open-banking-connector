// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation.PaymentInitialisation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation
{
    public static class DomesticPaymentContextInteractionExtensions
    {
        public static DomesticPaymentContext Data(this DomesticPaymentContext context, DomesticPayment value)
        {
            context.Data = value;
            return context;
        }

        public static DomesticPaymentContext ConsentId(this DomesticPaymentContext context, string value)
        {
            context.ConsentId = value;
            return context;
        }

        public static async Task<FluentResponse<DomesticPaymentResponse>> SubmitAsync(
            this DomesticPaymentContext context)
        {
            context.ArgNotNull(nameof(context));
            try
            {
                DomesticPayment domesticPayment = context.Data ?? new DomesticPayment
                {
                    ConsentId = context.ConsentId.ArgNotNullElseInvalidOp("ConsentId not specified")
                };
                IList<FluentResponseMessage> validationErrors = new PaymentRequestValidator()
                    .Validate(domesticPayment)
                    .GetOpenBankingResponses();
                if (validationErrors.Count > 0)
                {
                    return new FluentResponse<DomesticPaymentResponse>(validationErrors);
                }

                CreateDomesticPayment i = new CreateDomesticPayment(
                    apiClient: context.Context.ApiClient,
                    bankRepo: context.Context.BankRepository,
                    bankProfileRepo: context.Context.BankProfileRepository,
                    domesticConsentRepo: context.Context.DomesticConsentRepository,
                    mapper: context.Context.EntityMapper,
                    bankRegistrationRepo: context.Context.BankRegistrationRepository,
                    softwareStatementProfileService: context.Context.SoftwareStatementProfileService);

                DomesticPaymentResponse resp = await i.CreateAsync(domesticPayment);

                return new FluentResponse<DomesticPaymentResponse>(resp);
            }
            catch (AggregateException ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new FluentResponse<DomesticPaymentResponse>(ex.CreateErrorMessages());
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new FluentResponse<DomesticPaymentResponse>(ex.CreateErrorMessage());
            }
        }
    }
}
