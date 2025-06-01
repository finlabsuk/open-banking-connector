﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.Management;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

public class DanskeGenerator : BankProfileGeneratorBase<DanskeBank>
{
    public DanskeGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider) : base(
        bankProfilesSettingsProvider,
        BankGroup.Danske) { }

    //See https://developers.danskebank.com/documentation
    public override BankProfile GetBankProfile(
        DanskeBank bank,
        IInstrumentationClient instrumentationClient) =>
        new(
            _bankGroupData.GetBankProfile(bank),
            "https://sandbox-obp-api.danskebank.com/sandbox-open-banking/private", //from https://developers.danskebank.com/documentation#endpoints
            "0015800000jf7AeAAI", //from https://developers.danskebank.com/api_products/danske_bank_apis/pi?view=documentation
            null,
            null,
            new PaymentInitiationApi
            {
                BaseUrl =
                    "https://sandbox-obp-api.danskebank.com/sandbox-open-banking/v3.1/pisp" //from https://developers.danskebank.com/api_products/danske_bank_apis/pi?view=reference
            },
            null,
            null,
            null,
            false,
            instrumentationClient)
        {
            CustomBehaviour = new CustomBehaviourClass
            {
                BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                {
                    TransportCertificateSubjectDnOrgIdEncoding = SubjectDnOrgIdEncoding.DottedDecimalAttributeType,
                    UseApplicationJoseNotApplicationJwtContentTypeHeader = true
                },
                OpenIdConfigurationGet = new OpenIdConfigurationGetCustomBehaviour
                {
                    ResponseModesSupportedResponse = new List<OAuth2ResponseMode>
                    {
                        // missing from OpenID configuration
                        OAuth2ResponseMode.Fragment
                    }
                },
                DomesticPaymentConsentAuthGet = new ConsentAuthGetCustomBehaviour
                {
                    IdTokenProcessingCustomBehaviour =
                        new IdTokenProcessingCustomBehaviour { IdTokenMayNotHaveAuthTimeClaim = true }
                },
                DomesticPaymentConsentAuthCodeGrantPost = new AuthCodeGrantPostCustomBehaviour
                {
                    ExpectedResponseRefreshTokenMayBeAbsent = true,
                    IdTokenProcessingCustomBehaviour =
                        new IdTokenProcessingCustomBehaviour { IdTokenMayNotHaveAuthTimeClaim = true }
                },
                DomesticPaymentConsent =
                    new DomesticPaymentConsentCustomBehaviour { PreferMisspeltContractPresentIndicator = true },
                DomesticPayment = new DomesticPaymentCustomBehaviour
                {
                    PreferMisspeltContractPresentIndicator = true,
                    ResponseDataDebtorSchemeNameMayBeMissingOrWrong = true,
                    ResponseDataDebtorIdentificationMayBeMissingOrWrong = true,
                    ResponseDataRefundMayBeMissingOrWrong = true
                }
            },
            AspspBrandId = 10007 // sandbox
        };
}
