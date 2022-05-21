// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public partial class BankProfileDefinitions
    {
        public BankProfile Lloyds { get; }

        // See https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/LBG_Sandbox_Developers_Guide_V403.pdf
        // which includes:
        // API discovery endpoint (section 2.4): https://rs.aspsp.sandbox.lloydsbanking.com/open-banking/discover

        private BankProfile GetLloyds()
        {
            BankProfileHiddenProperties bankProfileHiddenProperties =
                GetRequiredBankProfileHiddenProperties(BankProfileEnum.Lloyds);
            return new BankProfile(
                BankProfileEnum.Lloyds,
                "https://as.aspsp.sandbox.lloydsbanking.com/oauth2", // from API discovery endpoint
                "0015800000jf9GgAAI", // from API discovery endpoint
                bankProfileHiddenProperties.GetRequiredClientRegistrationApiVersion(),
                null,
                new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion =
                        PaymentInitiationApiVersion.Version3p1p6, // from API discovery endpoint
                    BaseUrl =
                        "https://matls.rs.aspsp.sandbox.lloydsbanking.com/open-banking/v3.1.6/pisp" // from API discovery endpoint
                },
                null)
            {
                BankConfigurationApiSettings = new BankConfigurationApiSettings
                {
                    BankRegistrationAdjustments = registration =>
                    {
                        BankRegistrationPostCustomBehaviour bankRegistrationPostCustomBehaviour =
                            (registration.CustomBehaviour ??= new CustomBehaviourClass())
                            .BankRegistrationPost ??= new BankRegistrationPostCustomBehaviour();
                        bankRegistrationPostCustomBehaviour.GrantTypesClaim =
                            new List<ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum>
                            {
                                ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum
                                    .ClientCredentials,
                                ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum
                                    .AuthorizationCode
                            };
                        bankRegistrationPostCustomBehaviour.SubjectTypeClaim = "pairwise";

                        OpenIdConfigurationGetCustomBehaviour openIdConfigurationGetCustomBehaviour =
                            registration.CustomBehaviour
                                .OpenIdConfigurationGet ??= new OpenIdConfigurationGetCustomBehaviour();
                        openIdConfigurationGetCustomBehaviour.ResponseModesSupportedResponse =
                            new List<OAuth2ResponseMode>
                            {
                                // missing from OpenID configuration
                                OAuth2ResponseMode.Fragment
                            };

                        return registration;
                    }
                }
            };
        }
    }
}
