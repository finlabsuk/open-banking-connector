// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

public class MonzoGenerator : BankProfileGeneratorBase<MonzoBank>
{
    public MonzoGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider,
        IBankGroup<MonzoBank> bankGroup) : base(bankProfilesSettingsProvider, bankGroup) { }

    public override BankProfile GetBankProfile(MonzoBank bank)
    {
        return new BankProfile(
            _bankGroup.GetBankProfile(bank),
            "https://api.s101.nonprod-ffs.io/open-banking/", //from https://docs.monzo.com/#well-known-endpoints
            GetFinancialId(bank),
            null,
            new PaymentInitiationApi
            {
                ApiVersion = GetPaymentInitiationApiVersion(bank),
                BaseUrl =
                    "https://openbanking.s101.nonprod-ffs.io/open-banking/v3.1/pisp" //from https://docs.monzo.com/#well-known-endpoints58
            },
            null,
            false)
        {
            CustomBehaviour = new CustomBehaviourClass
            {
                BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                {
                    UseTransportCertificateSubjectDnWithDottedDecimalOrgIdAttribute = true,
                },
                ClientCredentialsGrantPost = new GrantPostCustomBehaviour
                {
                    DoNotValidateScopeResponse = true
                }
            },
            PaymentInitiationApiSettings = new PaymentInitiationApiSettings
            {
                DomesticPaymentConsentExternalApiRequestAdjustments = externalApiRequest =>
                {
                    externalApiRequest.Data.Initiation.SupplementaryData =
                        new Dictionary<string, object>
                        {
                            ["DesiredStatus"] = "Authorised",
                            ["UserID"] = "user_0000A4C4nqORb7K9YYW3r0",
                            ["AccountID"] = "acc_0000A4C4o66FCYJoERQhHN"
                        };
                    return externalApiRequest;
                }
            }
        };
    }
}
