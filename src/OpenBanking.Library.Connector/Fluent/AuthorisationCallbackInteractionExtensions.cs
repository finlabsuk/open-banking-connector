// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public static class AuthorisationCallbackInteractionExtensions
    {
        public static AuthorisationCallbackContext SetData(this AuthorisationCallbackContext context,
            AuthorisationCallbackData data)
        {
            context.ArgNotNull(nameof(context));

            context.Data = data;

            return context;
        }

        public static async Task<AuthorisationCallbackResponse> SubmitAsync(this AuthorisationCallbackContext context)
        {
            context.ArgNotNull(nameof(context));

            var validationErrors = Validate(context);
            if (validationErrors.Count > 0)
            {
                return new AuthorisationCallbackResponse(validationErrors);
            }

            try
            {
                var handler = new RedirectCallbackHandler(context.Context.SoftwareStatementRepository,
                    context.Context.ApiClient, context.Context.EntityMapper,
                    context.Context.ClientRepository, context.Context.ClientProfileRepository,
                    context.Context.DomesticConsentRepository);

                await handler.CreateAsync(context.Data);

                return new AuthorisationCallbackResponse(new List<OpenBankingResponseMessage>());
            }
            catch (AggregateException ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new AuthorisationCallbackResponse(ex.CreateErrorMessages());
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new AuthorisationCallbackResponse(new[] { ex.CreateErrorMessage() });
            }
        }


        private static IList<OpenBankingResponseMessage> Validate(AuthorisationCallbackContext context)
        {
            return new AuthorisationCallbackDataValidator()
                .Validate(context.Data)
                .GetOpenBankingResponses();
        }
    }
}
