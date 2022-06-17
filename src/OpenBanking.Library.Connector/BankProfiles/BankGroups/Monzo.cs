// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups
{
    public enum MonzoBank
    {
        Monzo
    }

    public class Monzo : BankGroupBase<MonzoBank>
    {
        protected override ConcurrentDictionary<BankProfileEnum, MonzoBank> BankProfileToBank { get; } =
            new()
            {
                [BankProfileEnum.Monzo] = MonzoBank.Monzo
            };

        public override BankProfile GetBankProfile(
            BankProfileEnum bankProfileEnum,
            HiddenPropertiesDictionary hiddenPropertiesDictionary)
        {
            MonzoBank bank = GetBank(bankProfileEnum);
            BankProfileHiddenProperties bankProfileHiddenProperties =
                hiddenPropertiesDictionary[bankProfileEnum] ??
                throw new Exception(
                    $"Hidden properties are required for bank profile {bankProfileEnum} and cannot be found.");
            return new BankProfile(
                bankProfileEnum,
                "https://api.s101.nonprod-ffs.io/open-banking/", //from https://docs.monzo.com/#well-known-endpoints
                bankProfileHiddenProperties.GetRequiredFinancialId(),
                null,
                new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion = bankProfileHiddenProperties
                        .GetRequiredPaymentInitiationApiVersion(),
                    BaseUrl =
                        "https://openbanking.s101.nonprod-ffs.io/open-banking/v3.1/pisp" //from https://docs.monzo.com/#well-known-endpoints58
                },
                null,
                false)
            {
                CustomBehaviour = new CustomBehaviourClass
                {
                    ClientCredentialsGrantPost = new GrantPostCustomBehaviour
                    {
                        DoNotValidateScopeResponse = true
                    }
                },
                PaymentInitiationApiSettings = new PaymentInitiationApiSettings
                {
                    DomesticPaymentConsentAdjustments = consent =>
                    {
                        consent.ExternalApiRequest.Data.Initiation.SupplementaryData =
                            new Dictionary<string, object>
                            {
                                ["DesiredStatus"] = "Authorised",
                                ["UserID"] = "user_0000A4C4nqORb7K9YYW3r0",
                                ["AccountID"] = "acc_0000A4C4o66FCYJoERQhHN"
                            };
                        return consent;
                    }
                }
            };
        }
    }
}
