// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups
{
    public enum HsbcBank
    {
        Sandbox,
        UkPersonal
    }

    public class Hsbc
    {
        public BankProfile GetBankProfile(
            HsbcBank bank,
            BankProfileHiddenPropertiesDictionary hiddenPropertiesDictionary)
        {
            BankProfileEnum bankProfileEnum = bank switch
            {
                HsbcBank.Sandbox => BankProfileEnum.Hsbc_Sandbox,
                HsbcBank.UkPersonal => BankProfileEnum.Hsbc_UkPersonal,
                _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
            };

            BankProfileHiddenProperties bankProfileHiddenProperties =
                hiddenPropertiesDictionary[bankProfileEnum] ??
                throw new Exception(
                    $"Hidden properties are required for bank profile {bankProfileEnum} and cannot be found.");

            return new BankProfile(
                bankProfileEnum,
                bankProfileHiddenProperties.GetRequiredIssuerUrl(),
                bankProfileHiddenProperties.GetRequiredFinancialId(),
                bankProfileHiddenProperties.GetRequiredClientRegistrationApiVersion(),
                new AccountAndTransactionApi
                {
                    AccountAndTransactionApiVersion = bankProfileHiddenProperties
                        .GetRequiredAccountAndTransactionApiVersion(),
                    BaseUrl = bankProfileHiddenProperties
                        .GetRequiredAccountAndTransactionApiBaseUrl()
                },
                null,
                null)
            {
                BankConfigurationApiSettings = new BankConfigurationApiSettings
                {
                    BankRegistrationAdjustments = registration =>
                    {
                        (registration.CustomBehaviour ??= new CustomBehaviour())
                            .UseApplicationJoseNotApplicationJwtContentTypeHeader = true;
                        (registration.CustomBehaviour.BankRegistrationClaimsOverrides ??=
                                new BankRegistrationClaimsOverrides())
                            .Audience = bankProfileHiddenProperties.GetAdditionalProperty2();
                        (registration.CustomBehaviour.BankRegistrationResponseJsonOptions ??=
                                new BankRegistrationResponseJsonOptions())
                            .ClientIdIssuedAtConverterOptions =
                            DateTimeOffsetToUnixConverterOptions.JsonUsesMilliSecondsNotSeconds;
                        return registration;
                    },
                },
                AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
                {
                    AccountAccessConsentAdjustments = consent =>
                    {
                        var elementsToRemove = new List<OBReadConsent1DataPermissionsEnum>
                        {
                            OBReadConsent1DataPermissionsEnum.ReadOffers,
                            OBReadConsent1DataPermissionsEnum.ReadPartyPSU,
                            OBReadConsent1DataPermissionsEnum.ReadStatementsBasic,
                            OBReadConsent1DataPermissionsEnum.ReadStatementsDetail,
                        };
                        foreach (OBReadConsent1DataPermissionsEnum element in elementsToRemove)
                        {
                            consent.ExternalApiRequest.Data.Permissions.Remove(element);
                        }

                        if (bankProfileEnum is BankProfileEnum.Hsbc_Sandbox)
                        {
                            consent.ExternalApiRequest.Data.ExpirationDateTime = DateTimeOffset.UtcNow.AddDays(89);
                        }

                        return consent;
                    }
                }
            };
        }
    }
}
