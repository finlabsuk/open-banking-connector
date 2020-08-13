// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation
{
    public static class PaymentInitiationApiProfileContextInteractionExtensions
    {
        public static PaymentInitiationApiProfileContext Data(
            this PaymentInitiationApiProfileContext context,
            PaymentInitiationApiProfile value)
        {
            context.Data = value;
            return context;
        }

        public static PaymentInitiationApiProfileContext Id(
            this PaymentInitiationApiProfileContext context,
            string value)
        {
            context.Id = value;
            return context;
        }

        public static PaymentInitiationApiProfileContext BankClientProfileId(
            this PaymentInitiationApiProfileContext context,
            string value)
        {
            context.BankClientProfileId = value;
            return context;
        }

        public static PaymentInitiationApiProfileContext PaymentInitiationApiInfo(
            this PaymentInitiationApiProfileContext context,
            ApiVersion apiVersion,
            string baseUrl)
        {
            context.ApiVersion = apiVersion;
            context.BaseUrl = baseUrl;
            return context;
        }

        public static async Task<PaymentInitiationApiProfileFluentResponse> SubmitAsync(
            this PaymentInitiationApiProfileContext context)
        {
            try
            {
                PaymentInitiationApiProfile apiProfile = context.Data ?? new PaymentInitiationApiProfile(
                    id: context.Id.ArgNotNullElseInvalidOp("Id not specified"),
                    bankClientProfileId: context.BankClientProfileId.ArgNotNullElseInvalidOp(
                        "BankClientProfileId not specified"),
                    apiVersion: context.ApiVersion.ArgStructNotNullElseInvalidOp(
                        "AccountTransactionApiInfo not specified"),
                    baseUrl: context.BaseUrl.ArgNotNullElseInvalidOp("AccountTransactionApiInfo not specified"));

                CreatePaymentInitiationApiProfile i = new CreatePaymentInitiationApiProfile(
                    dbContextService: context.Context.DbContextService,
                    apiProfileRepo: context.Context.ApiProfileRepository);

                PaymentInitiationApiProfileResponse resp = await i.CreateAsync(apiProfile);

                return new PaymentInitiationApiProfileFluentResponse(resp);
            }
            catch (AggregateException ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new PaymentInitiationApiProfileFluentResponse(messages: ex.CreateErrorMessages(), data: null);
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new PaymentInitiationApiProfileFluentResponse(message: ex.CreateErrorMessage(), data: null);
            }
        }
    }
}
