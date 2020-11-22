// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles
{
    public enum ClientRegistrationBehaviour
    {
        OverwritesExisting,
        CreatesNew,
        CreatesNewIfNoneElseFails,
        DynamicRegistrationNotSupported
    }

    public delegate BankRegistration BankRegistrationAdjustments(
        BankRegistration bankRegistration,
        RegistrationScopeApiSet registrationScopeApiSet);

    public delegate DomesticPaymentConsent DomesticPaymentConsentAdjustments(
        DomesticPaymentConsent domesticPaymentConsent);

    public partial class BankProfile
    {
        public BankProfile(
            BankProfileEnum bankProfileEnum,
            string issuerUrl,
            string financialId,
            ClientRegistrationBehaviour clientRegistrationBehaviour,
            PaymentInitiationApi? defaultPaymentInitiationApi,
            ISet<RegistrationScopeApi> supportedApisInRegistrationScope)
        {
            BankProfileEnum = bankProfileEnum;
            IssuerUrl = issuerUrl ?? throw new ArgumentNullException(nameof(issuerUrl));
            FinancialId = financialId ?? throw new ArgumentNullException(nameof(financialId));
            ClientRegistrationBehaviour = clientRegistrationBehaviour;
            DefaultPaymentInitiationApi = defaultPaymentInitiationApi;
            SupportedApisInRegistrationScope = supportedApisInRegistrationScope;
        }

        public BankProfileEnum BankProfileEnum { get; }

        public string IssuerUrl { get; }

        public string FinancialId { get; }

        public ClientRegistrationBehaviour ClientRegistrationBehaviour { get; }

        public PaymentInitiationApi? DefaultPaymentInitiationApi { get; }

        public ISet<RegistrationScopeApi> SupportedApisInRegistrationScope { get; }

        /// <summary>
        ///     Does bank allow a registration with multiple API scopes (e.g. AISP and PISP)
        /// </summary>
        public bool MultipleApiTypesRegistrationSupported { get; set; } = true;

        /// <summary>
        ///     Adjustments to default BankRegistration request object.
        /// </summary>
        public BankRegistrationAdjustments BankRegistrationAdjustments { get; set; } =
            (x, _) => x;

        public ISet<RegistrationScopeApiSet> ApiTypeSetsSupported
        {
            get
            {
                HashSet<RegistrationScopeApiSet> x = SupportedApisInRegistrationScope
                    .Select(RegistrationScopeApiHelper.ApiTypeSetWithSingleApiType)
                    .ToHashSet();
                if (MultipleApiTypesRegistrationSupported && ApiTypeSetForMultipleApiTypesRegistration.HasValue)
                {
                    x.Add(ApiTypeSetForMultipleApiTypesRegistration.Value);
                }

                return x;
            }
        }

        public DomesticPaymentConsentAdjustments
            DomesticPaymentConsentAdjustments { get; set; } = x => x;

        private RegistrationScopeApiSet? ApiTypeSetForMultipleApiTypesRegistration
        {
            get
            {
                if (SupportedApisInRegistrationScope.Count > 1)
                {
                    RegistrationScopeApiSet registrationScopeApiSet = RegistrationScopeApiSet.None;
                    foreach (RegistrationScopeApi apiType in SupportedApisInRegistrationScope)
                    {
                        registrationScopeApiSet =
                            registrationScopeApiSet | RegistrationScopeApiHelper.ApiTypeSetWithSingleApiType(apiType);
                    }

                    return registrationScopeApiSet;
                }

                return null;
            }
        }

        partial void CustomBankRegistrationResponsesPath(ref string path);
    }
}
