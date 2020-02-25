// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Validation.PaymentInitialisation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation
{
    public static class DomesticPaymentContextInteractionExtensions
    {
        private static readonly Lens<DomesticPaymentContext, DomesticPaymentRequest> DataLens =
            Lens.Create((DomesticPaymentContext c) => c.Data, (c, d) => c.Data = d);

        public static DomesticPaymentContext Data(this DomesticPaymentContext context, DomesticPaymentRequest value)
        {
            context.ArgNotNull(nameof(context));

            context.Data = value;

            return context;
        }

        public static DomesticPaymentContext ConsentId(this DomesticPaymentContext context, string value)
        {
            context.ArgNotNull(nameof(context));

            context.GetOrCreateDefault(DataLens).ConsentId = value;

            return context;
        }

        public static async Task<DomesticPaymentResponse> SubmitAsync(this DomesticPaymentContext context)
        {
            context.ArgNotNull(nameof(context));

            var validationErrors = Validate(context);
            if (validationErrors.Count > 0)
            {
                return new DomesticPaymentResponse(validationErrors);
            }

            try
            {
                var i = new CreateDomesticPayment(context.Context.ApiClient, context.Context.EntityMapper,
                    context.Context.SoftwareStatementRepository, context.Context.ClientProfileRepository,
                    context.Context.ClientRepository, context.Context.DomesticConsentRepository);

                var resp = await i.CreateAsync(
                    context.Data.ConsentId
                );

                return new DomesticPaymentResponse(resp.Data);
            }
            catch (AggregateException ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new DomesticPaymentResponse(ex.CreateErrorMessages());
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new DomesticPaymentResponse(ex.CreateErrorMessage());
            }
        }


        private static IList<OpenBankingResponseMessage> Validate(DomesticPaymentContext context)
        {
            return new PaymentRequestValidator()
                .Validate(context.Data)
                .GetOpenBankingResponses();
        }
    }
}
