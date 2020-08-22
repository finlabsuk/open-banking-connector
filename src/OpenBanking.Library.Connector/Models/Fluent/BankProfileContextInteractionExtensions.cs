// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public static class BankProfileContextInteractionExtensions
    {
        public static BankProfileContext Data(
            this BankProfileContext context,
            BankProfile value)
        {
            context.Data = value;
            return context;
        }

        public static BankProfileContext BankRegistrationId(
            this BankProfileContext context,
            string value)
        {
            context.Data.BankRegistrationId = value;
            return context;
        }

        public static BankProfileContext PaymentInitiationApiInfo(
            this BankProfileContext context,
            ApiVersion apiVersion,
            string baseUrl)
        {
            context.Data.PaymentInitiationApi = new PaymentInitiationApi
            {
                ApiVersion = apiVersion,
                BaseUrl = baseUrl
            };
            return context;
        }

        public static async Task<FluentResponse<BankProfileResponse>> SubmitAsync(this BankProfileContext context)
        {
            context.ArgNotNull(nameof(context));

            IList<FluentResponseMessage> validationErrors = Validate(context);
            if (validationErrors.Count > 0)
            {
                return new FluentResponse<BankProfileResponse>(messages: validationErrors, data: null);
            }

            try
            {
                CreateBankProfile i = new CreateBankProfile(
                    bankProfileRepo: context.Context.BankProfileRepository,
                    bankRegistrationRepo: context.Context.BankRegistrationRepository,
                    bankRepo: context.Context.BankRepository,
                    dbMultiEntityMethods: context.Context.DbContextService);

                BankProfileResponse resp = await i.CreateAsync(context.Data);

                return new FluentResponse<BankProfileResponse>(resp);
            }
            catch (AggregateException ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new FluentResponse<BankProfileResponse>(messages: ex.CreateErrorMessages(), data: null);
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new FluentResponse<BankProfileResponse>(message: ex.CreateErrorMessage(), data: null);
            }
        }

        private static IList<FluentResponseMessage> Validate(BankProfileContext context)
        {
            return new ApiProfileValidator()
                .Validate(context.Data)
                .GetOpenBankingResponses();
        }
    }
}
