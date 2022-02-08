// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.ClientRegistration;
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
                new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion =
                        PaymentInitiationApiVersion.Version3p1p6, // from API discovery endpoint
                    BaseUrl =
                        "https://matls.rs.aspsp.sandbox.lloydsbanking.com/open-banking/v3.1.6/pisp" // from API discovery endpoint
                },
                null)
            {
                ClientRegistrationApiSettings = new ClientRegistrationApiSettings
                {
                    BankRegistrationAdjustments = (registration, scope) =>
                    {
                        registration.BankRegistrationClaimsOverrides = new BankRegistrationClaimsOverrides
                        {
                            GrantTypes =
                                new List<ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum>
                                {
                                    ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum
                                        .ClientCredentials,
                                    ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum
                                        .AuthorizationCode
                                },
                            SubjectType = "pairwise"
                        };
                        registration.OpenIdConfigurationOverrides = new OpenIdConfigurationOverrides
                        {
                            ResponseModesSupported =
                                new List<string>
                                {
                                    "fragment", "query", "form_post"
                                }, // missing from OpenID response
                        };

                        return registration;
                    },
                }
            };
        }
    }
}