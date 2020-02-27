// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Validation;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Validation.PaymentInitialisation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation
{
    public static class DomesticPaymentConsentContextInteractionExtensions
    {
        private static readonly Lens<DomesticPaymentConsentContext, OBWriteDomesticConsent> ConsentLens =
            Lens.Create((DomesticPaymentConsentContext c) => c.Data, (c, d) => c.Data = d);

        private static readonly Lens<OBWriteDomesticConsent, OBWriteDomesticConsentData> ConsentDataLens =
            Lens.Create((OBWriteDomesticConsent c) => c.Data, (c, d) => c.Data = d);

        private static readonly Lens<OBWriteDomesticConsent, OBRisk> RiskLens =
            Lens.Create((OBWriteDomesticConsent c) => c.Risk, (c, d) => c.Risk = d);

        private static readonly Lens<OBWriteDomesticConsentData, OBWriteDomesticDataInitiation> InitiationLens =
            Lens.Create((OBWriteDomesticConsentData d) => d.Initiation, (c, d) => c.Initiation = d);

        private static readonly Lens<OBWriteDomesticDataInitiation, OBWriteDomesticDataInitiationRemittanceInformation>
            RemittanceLens = Lens.Create((OBWriteDomesticDataInitiation d) => d.RemittanceInformation,
                (c, d) => c.RemittanceInformation = d);

        private static readonly Lens<OBWriteDomesticDataInitiation, OBWriteDomesticDataInitiationCreditorAccount>
            CreditorAccountLens = Lens.Create((OBWriteDomesticDataInitiation d) => d.CreditorAccount,
                (c, d) => c.CreditorAccount = d);

        private static readonly Lens<OBWriteDomesticDataInitiation, OBWriteDomesticDataInitiationDebtorAccount>
            DebtorAccountLens = Lens.Create((OBWriteDomesticDataInitiation d) => d.DebtorAccount,
                (c, d) => c.DebtorAccount = d);

        private static readonly Lens<OBWriteDomesticDataInitiation, OBWriteDomesticDataInitiationInstructedAmount>
            InstructedAmountLens = Lens.Create((OBWriteDomesticDataInitiation d) => d.InstructedAmount,
                (c, d) => c.InstructedAmount = d);

        public static DomesticPaymentConsentContext Data(this DomesticPaymentConsentContext context,
            OBWriteDomesticConsent value)
        {
            context.ArgNotNull(nameof(context));
            value.ArgNotNull(nameof(value));

            context.Data = value;

            return context;
        }

        public static DomesticPaymentConsentContext ClientProfileId(this DomesticPaymentConsentContext context,
            string value)
        {
            context.ArgNotNull(nameof(context));
            value.ArgNotNull(nameof(value));

            context.OpenBankingClientProfileId = value;

            return context;
        }


        public static DomesticPaymentConsentContext OpenBankingClientProfileId(
            this DomesticPaymentConsentContext context, string value)
        {
            context.ArgNotNull(nameof(context));
            value.ArgNotNull(nameof(value));

            context.OpenBankingClientProfileId = value;

            return context;
        }

        // HACK: need better abstractions
        public static DomesticPaymentConsentContext DeliveryAddress(this DomesticPaymentConsentContext context,
            OBRiskDeliveryAddress value)
        {
            context.ArgNotNull(nameof(context));

            if (value != null)
            {
                context.GetOrCreateDefault(ConsentLens)
                    .GetOrCreateDefault(RiskLens).DeliveryAddress = value;
            }

            return context;
        }


        public static DomesticPaymentConsentContext Merchant(this DomesticPaymentConsentContext context,
            string merchantCategory, string merchantCustomerId, PaymentContextCode? paymentContextCode)
        {
            context.ArgNotNull(nameof(context));

            if (merchantCategory != null || merchantCustomerId != null || paymentContextCode.HasValue)
            {
                var risk = context.GetOrCreateDefault(ConsentLens)
                    .GetOrCreateDefault(RiskLens);

                risk.MerchantCategoryCode = merchantCategory;
                risk.MerchantCustomerIdentification = merchantCustomerId;
                risk.PaymentContextCode = paymentContextCode;
            }

            return context;
        }


        public static DomesticPaymentConsentContext InstructionIdentification(
            this DomesticPaymentConsentContext context, string value)
        {
            context.ArgNotNull(nameof(context));

            if (value != null)
            {
                context.GetOrCreateDefault(ConsentLens)
                    .GetOrCreateDefault(ConsentDataLens)
                    .GetOrCreateDefault(InitiationLens).InstructionIdentification = value;
            }

            return context;
        }


        public static DomesticPaymentConsentContext EndToEndIdentification(this DomesticPaymentConsentContext context,
            string value)
        {
            context.ArgNotNull(nameof(context));

            if (value != null)
            {
                context.GetOrCreateDefault(ConsentLens)
                    .GetOrCreateDefault(ConsentDataLens)
                    .GetOrCreateDefault(InitiationLens).EndToEndIdentification = value;
            }

            return context;
        }

        public static DomesticPaymentConsentContext Remittance(this DomesticPaymentConsentContext context,
            OBWriteDomesticDataInitiationRemittanceInformation value)
        {
            context.ArgNotNull(nameof(context));

            if (value != null)
            {
                context.GetOrCreateDefault(ConsentLens)
                    .GetOrCreateDefault(ConsentDataLens)
                    .GetOrCreateDefault(InitiationLens).RemittanceInformation = value;
            }

            return context;
        }

        public static DomesticPaymentConsentContext Remittance(this DomesticPaymentConsentContext context,
            string unstructured, string reference)
        {
            context.ArgNotNull(nameof(context));

            var remittance = context.GetOrCreateDefault(ConsentLens)
                .GetOrCreateDefault(ConsentDataLens)
                .GetOrCreateDefault(InitiationLens)
                .GetOrCreateDefault(RemittanceLens);

            remittance.Unstructured = unstructured;
            remittance.Reference = reference;

            return context;
        }

        public static DomesticPaymentConsentContext CreditorAccount(this DomesticPaymentConsentContext context,
            OBWriteDomesticDataInitiationCreditorAccount value)
        {
            context.ArgNotNull(nameof(context));
            if (value != null)
            {
                context.GetOrCreateDefault(ConsentLens)
                    .GetOrCreateDefault(ConsentDataLens)
                    .GetOrCreateDefault(InitiationLens).CreditorAccount = value;
            }

            return context;
        }

        public static DomesticPaymentConsentContext CreditorAccount(this DomesticPaymentConsentContext context,
            string identification, string schema, string name, string secondaryId)
        {
            context.ArgNotNull(nameof(context));

            var acct = context.GetOrCreateDefault(ConsentLens)
                .GetOrCreateDefault(ConsentDataLens)
                .GetOrCreateDefault(InitiationLens)
                .GetOrCreateDefault(CreditorAccountLens);


            acct.SchemeName = schema;
            acct.Identification = identification;
            acct.Name = name;
            acct.SecondaryIdentification = secondaryId;

            return context;
        }

        public static DomesticPaymentConsentContext DebtorAccount(this DomesticPaymentConsentContext context,
            OBWriteDomesticDataInitiationDebtorAccount value)
        {
            context.ArgNotNull(nameof(context));

            if (value != null)
            {
                context.GetOrCreateDefault(ConsentLens)
                    .GetOrCreateDefault(ConsentDataLens)
                    .GetOrCreateDefault(InitiationLens).DebtorAccount = value;
            }

            return context;
        }

        public static DomesticPaymentConsentContext DebtorAccount(this DomesticPaymentConsentContext context,
            string identification, string schema, string name, string secondaryId)
        {
            context.ArgNotNull(nameof(context));

            var acct = context.GetOrCreateDefault(ConsentLens)
                .GetOrCreateDefault(ConsentDataLens)
                .GetOrCreateDefault(InitiationLens)
                .GetOrCreateDefault(DebtorAccountLens);

            acct.Identification = identification;
            acct.Name = name;
            acct.SchemeName = schema;
            acct.SecondaryIdentification = secondaryId;

            return context;
        }

        public static DomesticPaymentConsentContext Amount(this DomesticPaymentConsentContext context,
            OBWriteDomesticDataInitiationInstructedAmount value)
        {
            context.ArgNotNull(nameof(context));
            if (value != null)
            {
                context.GetOrCreateDefault(ConsentLens)
                    .GetOrCreateDefault(ConsentDataLens)
                    .GetOrCreateDefault(InitiationLens).InstructedAmount = value;
            }

            return context;
        }

        public static DomesticPaymentConsentContext Amount(this DomesticPaymentConsentContext context, string currency,
            double value)
        {
            context.ArgNotNull(nameof(context));

            var amt = context.GetOrCreateDefault(ConsentLens)
                .GetOrCreateDefault(ConsentDataLens)
                .GetOrCreateDefault(InitiationLens)
                .GetOrCreateDefault(InstructedAmountLens);

            amt.Currency = currency;
            amt.Amount = value.ToString(CultureInfo.InvariantCulture);

            return context;
        }

        public static async Task<DomesticPaymentConsentResponse> SubmitAsync(this DomesticPaymentConsentContext context)
        {
            context.ArgNotNull(nameof(context));

            if (context.Data.OpenBankingClientProfileId == null)
            {
                context.Data.OpenBankingClientProfileId = context.OpenBankingClientProfileId;
            }

            var validationErrors = Validate(context);
            if (validationErrors.Count > 0)
            {
                return new DomesticPaymentConsentResponse(validationErrors);
            }

            try
            {
                var createDomesticConsent = new CreateDomesticPaymentConsent(
                    context.Context.EntityMapper,
                    context.Context.ApiClient,
                    context.Context.SoftwareStatementRepository,
                    context.Context.ClientProfileRepository,
                    context.Context.ClientRepository,
                    context.Context.DomesticConsentRepository
                );

                var result = await createDomesticConsent.CreateAsync(context.Data);

                return new DomesticPaymentConsentResponse(result);
            }
            catch (Exception ex)
            {
                context.Context.Instrumentation.Exception(ex);

                return new DomesticPaymentConsentResponse(ex.CreateErrorMessage());
            }
        }

        private static async Task<BankClientProfile> GetClientProfile(DomesticPaymentConsentContext context)
        {
            return await context.Context.ClientProfileRepository.GetAsync(context.OpenBankingClientProfileId);
        }

        private static IList<OpenBankingResponseMessage> Validate(DomesticPaymentConsentContext context)
        {
            return new OBWriteDomesticConsentValidator()
                .Validate(context.Data)
                .GetOpenBankingResponses();
        }
    }
}
