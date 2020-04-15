// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public static class AuthorisationCallbackDataInteractionExtensions
    {
        public static AuthorisationCallbackDataContext Data(this AuthorisationCallbackDataContext context, AuthorisationCallbackData value)
        {
            context.Data = value;
            return context;
        }

        public static AuthorisationCallbackDataContext ResponseMode(this AuthorisationCallbackDataContext context, string value)
        {
            context.ResponseMode = value;
            return context;
        }

        public static AuthorisationCallbackDataContext Response(this AuthorisationCallbackDataContext context, AuthorisationCallbackPayload value)
        {
            context.Response = value;
            return context;
        }

        public static async Task<AuthorisationCallbackDataFluentResponse> SubmitAsync(
            this AuthorisationCallbackDataContext context)
        {
            context.ArgNotNull(nameof(context));

            try
            {
                var authData = context.Data ?? new AuthorisationCallbackData(
                    context.ResponseMode.ArgNotNullElseInvalidOp("ResponseMode not specified"),
                    context.Response.ArgNotNullElseInvalidOp("Response not specified")
                );

                var validationErrors = new AuthorisationCallbackDataValidator()
                    .Validate(authData)
                    .GetOpenBankingResponses();
                if (validationErrors.Count > 0)
                {
                    return new AuthorisationCallbackDataFluentResponse(validationErrors);
                }

                var handler = new RedirectCallbackHandler(context.Context.SoftwareStatementRepository,
                    context.Context.ApiClient, context.Context.EntityMapper,
                    context.Context.ClientProfileRepository,
                    context.Context.DomesticConsentRepository);

                await handler.CreateAsync(authData);

                return new AuthorisationCallbackDataFluentResponse(new List<FluentResponseMessage>());
            }
            catch (AggregateException ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new AuthorisationCallbackDataFluentResponse(ex.CreateErrorMessages());
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new AuthorisationCallbackDataFluentResponse(new[] { ex.CreateErrorMessage() });
            }
        }
    }
}
