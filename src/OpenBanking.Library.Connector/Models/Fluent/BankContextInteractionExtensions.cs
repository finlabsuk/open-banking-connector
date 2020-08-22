// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public static class BankContextInteractionExtensions
    {
        public static BankContext Data(this BankContext context, Bank value)
        {
            context.ArgNotNull(nameof(context));
            value.ArgNotNull(nameof(value));

            context.Data = value;

            return context;
        }
        
        public static async Task<FluentResponse<BankResponse>> SubmitAsync(this BankContext context)
        {
            context.ArgNotNull(nameof(context));

            IList<FluentResponseMessage> validationErrors = Validate(context);
            if (validationErrors.Count > 0)
            {
                return new FluentResponse<BankResponse>(messages: validationErrors, data: null);
            }

            try
            {
                var i = new CreateBank(context.Context.BankRepository, context.Context.DbContextService);

                BankResponse resp = await i.CreateAsync(context.Data);

                return new FluentResponse<BankResponse>(resp);
            }
            catch (AggregateException ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new FluentResponse<BankResponse>(messages: ex.CreateErrorMessages(), data: null);
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new FluentResponse<BankResponse>(message: ex.CreateErrorMessage(), data: null);
            }
        }

        private static IList<FluentResponseMessage> Validate(BankContext context)
        {
            return new BankValidator()
                .Validate(context.Data)
                .GetOpenBankingResponses();
        }
    }
}
