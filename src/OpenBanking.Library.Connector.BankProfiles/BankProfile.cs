// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.ClientRegistration;
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
        RegistrationScope registrationScope);

    public delegate bool RegistrationScopeSupported(RegistrationScope registrationScope);

    public delegate DomesticPaymentConsent DomesticPaymentConsentAdjustments(
        DomesticPaymentConsent domesticPaymentConsent);

    public partial class BankProfile
    {
        public BankProfile(
            BankProfileEnum bankProfileEnum,
            string issuerUrl,
            string financialId,
            ClientRegistrationApiVersion defaultClientRegistrationApiVersion,
            ClientRegistrationBehaviour clientRegistrationBehaviour,
            PaymentInitiationApi? defaultPaymentInitiationApi,
            ISet<RegistrationScopeElement> registrationScopeElementsSupported)
        {
            BankProfileEnum = bankProfileEnum;
            IssuerUrl = issuerUrl ?? throw new ArgumentNullException(nameof(issuerUrl));
            FinancialId = financialId ?? throw new ArgumentNullException(nameof(financialId));
            ClientRegistrationBehaviour = clientRegistrationBehaviour;
            DefaultPaymentInitiationApi = defaultPaymentInitiationApi;
            RegistrationScopeElementsSupported = registrationScopeElementsSupported;
            DefaultClientRegistrationApiVersion =
                defaultClientRegistrationApiVersion;
            BankUser bankUser = new BankUser("", "");
            CustomBankUser(ref bankUser);
            BankUser = bankUser;
        }

        public BankProfileEnum BankProfileEnum { get; }

        public string IssuerUrl { get; }

        public string FinancialId { get; }

        public ClientRegistrationBehaviour ClientRegistrationBehaviour { get; }

        public ClientRegistrationApiVersion DefaultClientRegistrationApiVersion { get; }

        public PaymentInitiationApi? DefaultPaymentInitiationApi { get; }

        public ISet<RegistrationScopeElement> RegistrationScopeElementsSupported { get; }

        /// <summary>
        ///     Does bank allow a registration with multiple API scopes (e.g. AISP and PISP)
        /// </summary>
        public bool RegistrationScopeWithMultipleElementsSupported { get; set; } = true;

        /// <summary>
        ///     Allows supported registration scopes to be specified. This allows for scope support to be
        ///     limited and checked. For example banks may not allow both AISP and PISP in the same scope.
        /// </summary>
        public RegistrationScopeSupported RegistrationScopeSupported { get; set; } = registrationScope => true;

        /// <summary>
        ///     Adjustments to default BankRegistration request object.
        /// </summary>
        public BankRegistrationAdjustments BankRegistrationAdjustments { get; set; } =
            (x, _) => x;

        public ISet<RegistrationScope> ApiTypeSetsSupported
        {
            get
            {
                HashSet<RegistrationScope> x = RegistrationScopeElementsSupported
                    .Select(RegistrationScopeApiHelper.ApiTypeSetWithSingleApiType)
                    .ToHashSet();
                if (RegistrationScopeWithMultipleElementsSupported &&
                    ApiTypeSetForMultipleApiTypesRegistration.HasValue)
                {
                    x.Add(ApiTypeSetForMultipleApiTypesRegistration.Value);
                }

                return x;
            }
        }

        public DomesticPaymentConsentAdjustments
            DomesticPaymentConsentAdjustments { get; set; } = x => x;

        private RegistrationScope? ApiTypeSetForMultipleApiTypesRegistration
        {
            get
            {
                if (RegistrationScopeElementsSupported.Count > 1)
                {
                    var registrationScopeApiSet = RegistrationScope.None;
                    foreach (RegistrationScopeElement apiType in RegistrationScopeElementsSupported)
                    {
                        registrationScopeApiSet =
                            registrationScopeApiSet | RegistrationScopeApiHelper.ApiTypeSetWithSingleApiType(apiType);
                    }

                    return registrationScopeApiSet;
                }

                return null;
            }
        }

        public BankUser BankUser { get; }

        partial void CustomBankRegistrationResponsesPath(ref string path);

        partial void CustomBankUser(ref BankUser bankUserInfo);

    }
}
