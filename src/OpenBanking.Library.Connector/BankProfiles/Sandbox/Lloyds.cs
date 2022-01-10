// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public partial class BankProfileDefinitions
    {
        public BankProfile Lloyds { get; }

        private BankProfile GetLloyds()
        {
            BankProfileHiddenProperties bankProfileHiddenProperties =
                GetRequiredBankProfileHiddenProperties(BankProfileEnum.Lloyds);
            return new BankProfile(
                BankProfileEnum.Lloyds,
                bankProfileHiddenProperties.GetRequiredIssuerUrl(),
                bankProfileHiddenProperties.GetRequiredFinancialId(),
                bankProfileHiddenProperties.GetRequiredClientRegistrationApiVersion(),
                new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion = bankProfileHiddenProperties
                        .GetRequiredPaymentInitiationApiVersion(),
                    BaseUrl = bankProfileHiddenProperties
                        .GetRequiredPaymentInitiationApiBaseUrl()
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
