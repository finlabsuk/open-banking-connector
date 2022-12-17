// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;
using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

// Note API discovery endpoints for sandbox:
// https://rs.aspsp.sandbox.lloydsbanking.com/open-banking/discovery
// https://matls.rs.aspsp.sandbox.lloydsbanking.com/open-banking/discovery
public class LloydsGenerator : BankProfileGeneratorBase<LloydsBank>
{
    public LloydsGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider,
        IBankGroup<LloydsBank> bankGroup) : base(bankProfilesSettingsProvider, bankGroup) { }

    public override BankProfile GetBankProfile(LloydsBank bank)
    {
        return new BankProfile(
            _bankGroup.GetBankProfile(bank),
            bank switch
            {
                LloydsBank.Sandbox =>
                    "https://as.aspsp.sandbox.lloydsbanking.com/oauth2", // from API discovery endpoint
                LloydsBank.LloydsPersonal =>
                    "https://authorise-api.lloydsbank.co.uk/prod01/channel/mtls/lyds/personal", // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide.pdf?v=1667713942
                LloydsBank.LloydsBusiness =>
                    "https://authorise-api.lloydsbank.co.uk/prod01/channel/mtls/lyds/business", // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide
                LloydsBank.LloydsCommerical =>
                    "https://authorise-api.lloydsbank.co.uk/prod01/channel/mtls/lyds/commercial", // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide
                LloydsBank.HalifaxPersonal =>
                    "https://authorise-api.halifax-online.co.uk/prod01/channel/mtls/hfx/personal", // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide
                LloydsBank.BankOfScotlandPersonal =>
                    "https://authorise-api.bankofscotland.co.uk/prod01/channel/mtls/bos/personal", // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide
                LloydsBank.BankOfScotlandBusiness =>
                    "https://authorise-api.bankofscotland.co.uk/prod01/channel/mtls/bos/business", // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide
                LloydsBank.BankOfScotlandCommerical =>
                    "https://authorise-api.bankofscotland.co.uk/prod01/channel/mtls/bos/commercial", // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide
                LloydsBank.MbnaPersonal =>
                    "https://authorise-api.mbna.co.uk/prod01/channel/mtls/mbn/personal", // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide
                _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
            },
            "0015800000jf9GgAAI", // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide_dcr.pdf?v=1631615723
            bank is not LloydsBank.Sandbox ? GetAccountAndTransactionApi(bank) : null,
            bank is LloydsBank.Sandbox
                ? new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion =
                        PaymentInitiationApiVersion.Version3p1p6, // from API discovery endpoint
                    BaseUrl =
                        "https://matls.rs.aspsp.sandbox.lloydsbanking.com/open-banking/v3.1.10/pisp" // from API discovery endpoint
                }
                : null,
            null,
            bank is not LloydsBank.Sandbox)
        {
            DynamicClientRegistrationApiVersion = DynamicClientRegistrationApiVersion
                .Version3p2, // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide_dcr.pdf?v=1631615723
            CustomBehaviour = new CustomBehaviourClass
            {
                BankRegistrationPost = bank is LloydsBank.Sandbox
                    ? new BankRegistrationPostCustomBehaviour
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
                    }
                    : new BankRegistrationPostCustomBehaviour
                    {
                        UseTransportCertificateDnWithStringNotHexDottedDecimalAttributeValues = true,
                        GrantTypesClaim =
                            new List<OBRegistrationProperties1grantTypesItemEnum>
                            {
                                OBRegistrationProperties1grantTypesItemEnum
                                    .ClientCredentials,
                                OBRegistrationProperties1grantTypesItemEnum
                                    .AuthorizationCode
                            },
                        UseApplicationJoseNotApplicationJwtContentTypeHeader =
                            true // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide_dcr.pdf?v=1631615723
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
            {
                UseRegistrationDeleteEndpoint = bank is LloydsBank.Sandbox,
                UseRegistrationAccessToken = bank is LloydsBank.Sandbox,
                BankRegistrationGroup = bank switch
                {
                    LloydsBank.Sandbox => BankRegistrationGroup.Lloyds_Sandbox,
                    LloydsBank.LloydsPersonal
                        or LloydsBank.LloydsBusiness
                        or LloydsBank.LloydsCommerical => BankRegistrationGroup.Lloyds_LloydsProduction,
                    LloydsBank.HalifaxPersonal => BankRegistrationGroup.Lloyds_HalifaxProduction,
                    LloydsBank.BankOfScotlandPersonal
                        or LloydsBank.BankOfScotlandBusiness
                        or LloydsBank.BankOfScotlandCommerical => BankRegistrationGroup.Lloyds_BankOfScotlandProduction,
                    LloydsBank.MbnaPersonal => BankRegistrationGroup.Lloyds_MbnaProduction,
                    _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
                }
            },
            AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
            {
                AccountAccessConsentExternalApiRequestAdjustments = externalApiRequest =>
                {
                    var elementsToRemove = new List<OBReadConsent1DataPermissionsEnum>
                    {
                        OBReadConsent1DataPermissionsEnum.ReadPAN
                    };
                    foreach (OBReadConsent1DataPermissionsEnum element in elementsToRemove)
                    {
                        externalApiRequest.Data.Permissions.Remove(element);
                    }

                    return externalApiRequest;
                }
            }
        };
    }

    private AccountAndTransactionApi GetAccountAndTransactionApi(LloydsBank bank)
    {
        return new AccountAndTransactionApi
        {
            AccountAndTransactionApiVersion =
                AccountAndTransactionApiVersion
                    .Version3p1p10, // from https://developer.lloydsbanking.com/node/4045
            BaseUrl =
                bank switch
                {
                    LloydsBank.Sandbox =>
                        "https://matls.rs.aspsp.sandbox.lloydsbanking.com/open-banking/v3.1.10/aisp", // from API discovery endpoint
                    LloydsBank.LloydsPersonal or LloydsBank.LloydsBusiness or LloydsBank.LloydsCommerical =>
                        "https://secure-api.lloydsbank.com/prod01/lbg/lyds/open-banking/v3.1/aisp", // from https://developer.lloydsbanking.com/node/4045
                    LloydsBank.HalifaxPersonal =>
                        "https://secure-api.halifax.co.uk/prod01/lbg/lyds/open-banking/v3.1/aisp", // from https://developer.lloydsbanking.com/node/4045
                    LloydsBank.BankOfScotlandPersonal or LloydsBank.BankOfScotlandBusiness
                        or LloydsBank.BankOfScotlandCommerical =>
                        "https://secure-api.bankofscotland.co.uk/prod01/lbg/lyds/open-banking/v3.1/aisp", // from https://developer.lloydsbanking.com/node/4045
                    LloydsBank.MbnaPersonal =>
                        "https://secure-api.mbna.co.uk/prod01/lbg/lyds/open-banking/v3.1/aisp", // from https://developer.lloydsbanking.com/node/4045
                    _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
                }
        };
    }
}
