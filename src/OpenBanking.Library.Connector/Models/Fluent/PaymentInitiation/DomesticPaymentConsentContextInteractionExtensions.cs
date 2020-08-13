// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validation.PaymentInitialisation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation
{
    public static class DomesticPaymentConsentContextInteractionExtensions
    {
        private static readonly Lens<DomesticPaymentConsentContext, DomesticPaymentConsent> BaseLens =
            Lens.Create(get: (DomesticPaymentConsentContext c) => c.Data, set: (c, d) => c.Data = d);

        private static readonly Lens<DomesticPaymentConsent, OBWriteDomesticConsent4> DomesticConsentLens =
            Lens.Create(get: (DomesticPaymentConsent c) => c.DomesticConsent, set: (c, d) => c.DomesticConsent = d);

        private static readonly Lens<OBWriteDomesticConsent4, OBWriteDomesticConsent4Data> DataLens =
            Lens.Create(get: (OBWriteDomesticConsent4 c) => c.Data, set: (c, d) => c.Data = d);

        private static readonly Lens<OBWriteDomesticConsent4, OBRisk1> RiskLens =
            Lens.Create(get: (OBWriteDomesticConsent4 c) => c.Risk, set: (c, d) => c.Risk = d);

        private static readonly Lens<OBWriteDomesticConsent4Data, OBWriteDomestic2DataInitiation> InitiationLens =
            Lens.Create(get: (OBWriteDomesticConsent4Data d) => d.Initiation, set: (c, d) => c.Initiation = d);

        private static readonly Lens<OBWriteDomestic2DataInitiation, OBWriteDomestic2DataInitiationCreditorAccount>
            CreditorAccountLens = Lens.Create(
                get: (OBWriteDomestic2DataInitiation d) => d.CreditorAccount,
                set: (c, d) => c.CreditorAccount = d);

        private static readonly Lens<OBWriteDomestic2DataInitiation, OBWriteDomestic2DataInitiationDebtorAccount>
            DebtorAccountLens = Lens.Create(
                get: (OBWriteDomestic2DataInitiation d) => d.DebtorAccount,
                set: (c, d) => c.DebtorAccount = d);

        private static readonly Lens<OBWriteDomestic2DataInitiation, OBWriteDomestic2DataInitiationInstructedAmount>
            InstructedAmountLens = Lens.Create(
                get: (OBWriteDomestic2DataInitiation d) => d.InstructedAmount,
                set: (c, d) => c.InstructedAmount = d);

        public static DomesticPaymentConsentContext Data(
            this DomesticPaymentConsentContext context,
            DomesticPaymentConsent value)
        {
            context.ArgNotNull(nameof(context));
            value.ArgNotNull(nameof(value));

            context.Data = value;

            return context;
        }

        public static DomesticPaymentConsentContext ApiProfileId(
            this DomesticPaymentConsentContext context,
            string value)
        {
            context.ArgNotNull(nameof(context));
            value.ArgNotNull(nameof(value));

            context.GetOrCreateDefault(BaseLens)
                .ApiProfileId = value;

            return context;
        }

        // HACK: need better abstractions
        public static DomesticPaymentConsentContext DeliveryAddress(
            this DomesticPaymentConsentContext context,
            OBRisk1DeliveryAddress value)
        {
            context.ArgNotNull(nameof(context));

            if (value != null)
            {
                context.GetOrCreateDefault(BaseLens)
                    .GetOrCreateDefault(DomesticConsentLens)
                    .GetOrCreateDefault(RiskLens).DeliveryAddress = value;
            }

            return context;
        }


        public static DomesticPaymentConsentContext Merchant(
            this DomesticPaymentConsentContext context,
            string merchantCategory,
            string merchantCustomerId,
            OBRisk1.PaymentContextCodeEnum? paymentContextCode)
        {
            context.ArgNotNull(nameof(context));

            if (merchantCategory != null || merchantCustomerId != null || paymentContextCode != null)
            {
                OBRisk1 risk = context.GetOrCreateDefault(BaseLens)
                    .GetOrCreateDefault(DomesticConsentLens)
                    .GetOrCreateDefault(RiskLens);

                risk.MerchantCategoryCode = merchantCategory;
                risk.MerchantCustomerIdentification = merchantCustomerId;
                risk.PaymentContextCode = paymentContextCode;
            }

            return context;
        }


        public static DomesticPaymentConsentContext InstructionIdentification(
            this DomesticPaymentConsentContext context,
            string value)
        {
            context.ArgNotNull(nameof(context));

            if (value != null)
            {
                context.GetOrCreateDefault(BaseLens)
                    .GetOrCreateDefault(DomesticConsentLens)
                    .GetOrCreateDefault(DataLens)
                    .GetOrCreateDefault(InitiationLens).InstructionIdentification = value;
            }

            return context;
        }


        public static DomesticPaymentConsentContext EndToEndIdentification(
            this DomesticPaymentConsentContext context,
            string value)
        {
            context.ArgNotNull(nameof(context));

            if (value != null)
            {
                context.GetOrCreateDefault(BaseLens)
                    .GetOrCreateDefault(DomesticConsentLens)
                    .GetOrCreateDefault(DataLens)
                    .GetOrCreateDefault(InitiationLens).EndToEndIdentification = value;
            }

            return context;
        }

        public static DomesticPaymentConsentContext Remittance(
            this DomesticPaymentConsentContext context,
            OBWriteDomestic2DataInitiationRemittanceInformation value)
        {
            context.ArgNotNull(nameof(context));

            if (value != null)
            {
                context.GetOrCreateDefault(BaseLens)
                    .GetOrCreateDefault(DomesticConsentLens)
                    .GetOrCreateDefault(DataLens)
                    .GetOrCreateDefault(InitiationLens).RemittanceInformation = value;
            }

            return context;
        }

        public static DomesticPaymentConsentContext CreditorAccount(
            this DomesticPaymentConsentContext context,
            OBWriteDomestic2DataInitiationCreditorAccount value)
        {
            context.ArgNotNull(nameof(context));
            if (value != null)
            {
                context.GetOrCreateDefault(BaseLens)
                    .GetOrCreateDefault(DomesticConsentLens)
                    .GetOrCreateDefault(DataLens)
                    .GetOrCreateDefault(InitiationLens).CreditorAccount = value;
            }

            return context;
        }

        public static DomesticPaymentConsentContext CreditorAccount(
            this DomesticPaymentConsentContext context,
            string identification,
            string schema,
            string name,
            string secondaryId)
        {
            context.ArgNotNull(nameof(context));

            OBWriteDomestic2DataInitiationCreditorAccount acct = context.GetOrCreateDefault(BaseLens)
                .GetOrCreateDefault(DomesticConsentLens)
                .GetOrCreateDefault(DataLens)
                .GetOrCreateDefault(InitiationLens)
                .GetOrCreateDefault(CreditorAccountLens);


            acct.SchemeName = schema;
            acct.Identification = identification;
            acct.Name = name;
            acct.SecondaryIdentification = secondaryId;

            return context;
        }

        public static DomesticPaymentConsentContext DebtorAccount(
            this DomesticPaymentConsentContext context,
            OBWriteDomestic2DataInitiationDebtorAccount value)
        {
            context.ArgNotNull(nameof(context));

            if (value != null)
            {
                context.GetOrCreateDefault(BaseLens)
                    .GetOrCreateDefault(DomesticConsentLens)
                    .GetOrCreateDefault(DataLens)
                    .GetOrCreateDefault(InitiationLens).DebtorAccount = value;
            }

            return context;
        }

        public static DomesticPaymentConsentContext DebtorAccount(
            this DomesticPaymentConsentContext context,
            string identification,
            string schema,
            string name,
            string secondaryId)
        {
            context.ArgNotNull(nameof(context));

            OBWriteDomestic2DataInitiationDebtorAccount acct = context.GetOrCreateDefault(BaseLens)
                .GetOrCreateDefault(DomesticConsentLens)
                .GetOrCreateDefault(DataLens)
                .GetOrCreateDefault(InitiationLens)
                .GetOrCreateDefault(DebtorAccountLens);

            acct.Identification = identification;
            acct.Name = name;
            acct.SchemeName = schema;
            acct.SecondaryIdentification = secondaryId;

            return context;
        }

        public static DomesticPaymentConsentContext Amount(
            this DomesticPaymentConsentContext context,
            OBWriteDomestic2DataInitiationInstructedAmount value)
        {
            context.ArgNotNull(nameof(context));
            if (value != null)
            {
                context.GetOrCreateDefault(BaseLens)
                    .GetOrCreateDefault(DomesticConsentLens)
                    .GetOrCreateDefault(DataLens)
                    .GetOrCreateDefault(InitiationLens).InstructedAmount = value;
            }

            return context;
        }

        public static DomesticPaymentConsentContext Amount(
            this DomesticPaymentConsentContext context,
            string currency,
            double value)
        {
            context.ArgNotNull(nameof(context));

            OBWriteDomestic2DataInitiationInstructedAmount amt = context.GetOrCreateDefault(BaseLens)
                .GetOrCreateDefault(DomesticConsentLens)
                .GetOrCreateDefault(DataLens)
                .GetOrCreateDefault(InitiationLens)
                .GetOrCreateDefault(InstructedAmountLens);

            amt.Currency = currency;
            amt.Amount = value.ToString(CultureInfo.InvariantCulture);

            return context;
        }

        public static async Task<DomesticPaymentConsentFluentResponse> SubmitAsync(
            this DomesticPaymentConsentContext context)
        {
            context.ArgNotNull(nameof(context));

            IList<FluentResponseMessage> validationErrors = Validate(context);
            if (validationErrors.Count > 0)
            {
                return new DomesticPaymentConsentFluentResponse(validationErrors);
            }

            try
            {
                CreateDomesticPaymentConsent createDomesticConsent = new CreateDomesticPaymentConsent(
                    apiClient: context.Context.ApiClient,
                    mapper: context.Context.EntityMapper,
                    dbMultiEntityMethods: context.Context.DbContextService,
                    bankClientProfileRepo: context.Context.ClientProfileRepository,
                    apiProfileRepo: context.Context.ApiProfileRepository,
                    domesticConsentRepo: context.Context.DomesticConsentRepository,
                    softwareStatementProfileService: context.Context.SoftwareStatementProfileService);

                PaymentConsentResponse result = await createDomesticConsent.CreateAsync(context.Data);

                return new DomesticPaymentConsentFluentResponse(result);
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new DomesticPaymentConsentFluentResponse(ex.CreateErrorMessage());
            }
        }

        private static IList<FluentResponseMessage> Validate(DomesticPaymentConsentContext context)
        {
            return new OBWriteDomesticConsentValidator()
                .Validate(context.Data)
                .GetOpenBankingResponses();
        }
    }
}
