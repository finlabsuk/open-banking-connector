﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public partial class BankProfileDefinitions
    {
        public BankProfile Hsbc { get; }

        private BankProfile GetHsbc()
        {
            BankProfileHiddenProperties bankProfileHiddenProperties =
                GetRequiredBankProfileHiddenProperties(BankProfileEnum.Hsbc);
            return new BankProfile(
                BankProfileEnum.Hsbc,
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
                ClientRegistrationApiSettings = new ClientRegistrationApiSettings
                {
                    BankRegistrationAdjustments = (registration, scope) =>
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

                        return consent;
                    }
                }
            };
        }
    }
}