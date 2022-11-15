// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators
{
    // See https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/LBG_Sandbox_Developers_Guide_V403.pdf
    // which includes:
    // API discovery endpoint (section 2.4): https://rs.aspsp.sandbox.lloydsbanking.com/open-banking/discover
    public class LloydsGenerator : BankProfileGeneratorBase<LloydsBank>
    {
        public LloydsGenerator(
            ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider,
            IBankGroup<LloydsBank> bankGroup) : base(bankProfilesSettingsProvider, bankGroup) { }

        public override BankProfile GetBankProfile(LloydsBank bank)
        {
            return new BankProfile(
                _bankGroup.GetBankProfile(bank),
                "https://as.aspsp.sandbox.lloydsbanking.com/oauth2", // from API discovery endpoint
                "0015800000jf9GgAAI", // from API discovery endpoint
                null,
                new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion =
                        PaymentInitiationApiVersion.Version3p1p6, // from API discovery endpoint
                    BaseUrl =
                        "https://matls.rs.aspsp.sandbox.lloydsbanking.com/open-banking/v3.1.10/pisp" // from API discovery endpoint
                },
                null,
                false)
            {
                CustomBehaviour = new CustomBehaviourClass
                {
                    BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                    {
                        GrantTypesClaim =
                            new List<OBRegistrationProperties1grantTypesItemEnum>
                            {
                                OBRegistrationProperties1grantTypesItemEnum
                                    .ClientCredentials,
                                OBRegistrationProperties1grantTypesItemEnum
                                    .AuthorizationCode
                            },
                        SubjectTypeClaim = "pairwise"
                    },
                    OpenIdConfigurationGet = new OpenIdConfigurationGetCustomBehaviour
                    {
                        ResponseModesSupportedResponse = new List<OAuth2ResponseMode>
                        {
                            // missing from OpenID configuration
                            OAuth2ResponseMode.Fragment
                        }
                    }
                },
                BankConfigurationApiSettings = new BankConfigurationApiSettings
                    { UseRegistrationDeleteEndpoint = true, UseRegistrationAccessToken = true }
            };
        }
    }
}
