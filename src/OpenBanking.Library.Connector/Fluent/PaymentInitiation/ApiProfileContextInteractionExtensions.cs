// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.Request.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation
{
    public static class ApiProfileContextInteractionExtensions
    {
        public static ApiProfileContext Data(this ApiProfileContext context, ApiProfile value)
        {
            context.Data = value;
            return context;
        }

        public static ApiProfileContext Id(this ApiProfileContext context, string value)
        {
            context.Id = value;
            return context;
        }

        public static ApiProfileContext BankClientProfileId(this ApiProfileContext context, string value)
        {
            context.BankClientProfileId = value;
            return context;
        }

        public static ApiProfileContext PaymentInitiationApiInfo(this ApiProfileContext context,
            ApiVersion apiVersion,
            string baseUrl
        )
        {
            context.ApiVersion = apiVersion;
            context.BaseUrl = baseUrl;
            return context;
        }

        public static async Task<ApiProfileResponse> SubmitAsync(this ApiProfileContext context)
        {
            try
            {
                var apiProfile = context.Data ?? new ApiProfile(
                    context.Id.ArgNotNullElseInvalidOp("Id not specified"),
                    context.BankClientProfileId.ArgNotNullElseInvalidOp(
                        "BankClientProfileId not specified"),
                    context.ApiVersion.ArgStructNotNullElseInvalidOp(
                        "AccountTransactionApiInfo not specified"),
                    context.BaseUrl.ArgNotNullElseInvalidOp("AccountTransactionApiInfo not specified")
                );

                var i = new CreateApiProfile(
                    context.Context.ApiClient,
                    context.Context.SoftwareStatementRepository,
                    context.Context.ClientProfileRepository,
                    context.Context.ClientRepository,
                    context.Context.ApiProfileRepository
                );

                var resp = await i.CreateAsync(apiProfile);

                return new ApiProfileResponse(resp);
            }
            catch (AggregateException ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new ApiProfileResponse(ex.CreateErrorMessages(), null);
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new ApiProfileResponse(ex.CreateErrorMessage(), null);
            }
        }
    }
}
