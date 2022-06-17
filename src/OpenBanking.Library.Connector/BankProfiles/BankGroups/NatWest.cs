// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups
{
    public enum NatWestBank
    {
        NatWest,
        RoyalBankOfScotland
    }

    public class NatWest : BankGroupBase<NatWestBank>
    {
        protected override ConcurrentDictionary<BankProfileEnum, NatWestBank> BankProfileToBank { get; } =
            new()
            {
                [BankProfileEnum.NatWest] = NatWestBank.NatWest,
                [BankProfileEnum.RoyalBankOfScotland] = NatWestBank.RoyalBankOfScotland
            };

        public override BankProfile GetBankProfile(
            BankProfileEnum bankProfileEnum,
            HiddenPropertiesDictionary hiddenPropertiesDictionary)
        {
            NatWestBank bank = GetBank(bankProfileEnum);
            BankProfileHiddenProperties bankProfileHiddenProperties =
                hiddenPropertiesDictionary[bankProfileEnum] ??
                throw new Exception(
                    $"Hidden properties are required for bank profile {bankProfileEnum} and cannot be found.");
            // var sandboxGrantPostCustomBehaviour = new GrantPostCustomBehaviour
            // {
            //     IdTokenSubClaimIsClientIdNotConsentId = true
            // };
            return new BankProfile(
                bankProfileEnum,
                bankProfileHiddenProperties.GetRequiredIssuerUrl(),
                bank switch
                {
                    NatWestBank
                        .NatWest => "0015800000jfwxXAAQ", // from https://bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                    NatWestBank.RoyalBankOfScotland => bankProfileHiddenProperties.GetRequiredFinancialId(),
                    _ => throw new ArgumentOutOfRangeException()
                },
                null,
                new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion =
                        PaymentInitiationApiVersion
                            .Version3p1p6, // from https://www.bankofapis.com/products/natwest-group-open-banking/payments/documentation/nwb/3.1.6
                    BaseUrl = bankProfileHiddenProperties
                        .GetRequiredPaymentInitiationApiBaseUrl()
                },
                null,
                false)
            {
                CustomBehaviour = new CustomBehaviourClass
                {
                    // HttpMtls = bank is NatWestBank.NatWest
                    //     ? new HttpMtlsCustomBehaviour { DisableTlsCertificateVerification = true }
                    //     : null,
                    BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                    {
                        ScopeClaimResponseJsonConverter =
                            DelimitedStringConverterOptions.JsonIsStringArrayNotString
                    },
                    // DomesticPaymentConsentAuthGet = new ConsentAuthGetCustomBehaviour
                    // {
                    //     IdTokenSubClaimIsClientIdNotConsentId = true
                    // },
                    // AuthCodeGrantPost = sandboxGrantPostCustomBehaviour,
                    // RefreshTokenGrantPost = sandboxGrantPostCustomBehaviour
                }
            };
        }
    }
}
