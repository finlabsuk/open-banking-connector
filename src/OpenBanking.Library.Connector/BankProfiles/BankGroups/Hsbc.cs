// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups
{
    public enum HsbcBank
    {
        FirstDirect,
        Sandbox,
        UkBusiness,
        UkKinetic,
        UkPersonal
    }

    public class Hsbc : BankGroupBase<HsbcBank>
    {
        protected override ConcurrentDictionary<BankProfileEnum, HsbcBank> BankProfileToBank { get; } =
            new()
            {
                [BankProfileEnum.Hsbc_FirstDirect] = HsbcBank.FirstDirect,
                [BankProfileEnum.Hsbc_Sandbox] = HsbcBank.Sandbox,
                [BankProfileEnum.Hsbc_UkBusiness] = HsbcBank.UkBusiness,
                [BankProfileEnum.Hsbc_UkKinetic] = HsbcBank.UkKinetic,
                [BankProfileEnum.Hsbc_UkPersonal] = HsbcBank.UkPersonal
            };

        public override BankProfile GetBankProfile(
            BankProfileEnum bankProfileEnum,
            HiddenPropertiesDictionary hiddenPropertiesDictionary)
        {
            HsbcBank bank = GetBank(bankProfileEnum);
            BankProfileHiddenProperties bankProfileHiddenProperties =
                hiddenPropertiesDictionary[bankProfileEnum] ??
                throw new Exception(
                    $"Hidden properties are required for bank profile {bankProfileEnum} and cannot be found.");
            (string issuerUrl, string accountAndTransactionApiBaseUrl) = bank switch
            {
                HsbcBank.FirstDirect => (
                    // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/915047304/Implementation+Guide+first+direct
                    "https://api.ob.firstdirect.com",
                    "https://api.ob.firstdirect.com/obie/open-banking/v3.1/aisp"),
                HsbcBank.Sandbox => (
                    bankProfileHiddenProperties.GetRequiredIssuerUrl(),
                    bankProfileHiddenProperties.GetRequiredAccountAndTransactionApiBaseUrl()),
                HsbcBank.UkBusiness => (
                    // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/1059489023/Implementation+Guide+HSBC+Business
                    "https://api.ob.business.hsbc.co.uk",
                    "https://api.ob.business.hsbc.co.uk/obie/open-banking/v3.1/aisp"),
                HsbcBank.UkKinetic => (
                    // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/1387201093/Implementation+Guide+HSBC+-+Kinetic
                    "https://api.ob.hsbckinetic.co.uk",
                    "https://api.ob.hsbckinetic.co.uk/obie/open-banking/v3.1/aisp"),
                HsbcBank.UkPersonal => (
                    // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/108266712/Implementation+Guide+HSBC+Personal
                    "https://api.ob.hsbc.co.uk",
                    "https://api.ob.hsbc.co.uk/obie/open-banking/v3.1/aisp"),
                _ => throw new ArgumentOutOfRangeException()
            };
            var sandboxGrantPostCustomBehaviour = new GrantPostCustomBehaviour
            {
                DoNotValidateIdToken = true
            };
            return new BankProfile(
                bankProfileEnum,
                issuerUrl,
                bankProfileHiddenProperties.GetRequiredFinancialId(),
                new AccountAndTransactionApi
                {
                    AccountAndTransactionApiVersion = bankProfileHiddenProperties
                        .GetRequiredAccountAndTransactionApiVersion(),
                    BaseUrl = accountAndTransactionApiBaseUrl
                },
                null,
                null,
                bank is not HsbcBank.Sandbox)
            {
                CustomBehaviour = new CustomBehaviourClass
                {
                    OpenIdConfigurationGet = bank is HsbcBank.Sandbox
                        ? new OpenIdConfigurationGetCustomBehaviour
                        {
                            Url = bankProfileHiddenProperties.GetAdditionalProperty1()
                        }
                        : null,
                    BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                    {
                        ClientIdIssuedAtClaimResponseJsonConverter = DateTimeOffsetConverter.UnixMilliSecondsJsonFormat,
                        AudClaim = issuerUrl,
                        UseApplicationJoseNotApplicationJwtContentTypeHeader = true
                    },
                    JwksGet = bank is HsbcBank.Sandbox
                        ? new JwksGetCustomBehaviour
                        {
                            ResponseHasNoRootProperty = true
                        }
                        : null,
                    AccountAccessConsentAuthGet = bank is HsbcBank.Sandbox
                        ? new ConsentAuthGetCustomBehaviour
                        {
                            DoNotValidateIdToken = true
                        }
                        : null,
                    AuthCodeGrantPost = bank is HsbcBank.Sandbox
                        ? sandboxGrantPostCustomBehaviour
                        : null,
                    RefreshTokenGrantPost = bank is HsbcBank.Sandbox
                        ? sandboxGrantPostCustomBehaviour
                        : null
                },
                AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
                {
                    ExternalApiRequestAdjustments = externalApiRequest =>
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
                            externalApiRequest.Data.Permissions.Remove(element);
                        }

                        if (bank is HsbcBank.Sandbox)
                        {
                            externalApiRequest.Data.ExpirationDateTime = DateTimeOffset.UtcNow.AddDays(89);
                        }

                        return externalApiRequest;
                    }
                },
                DefaultResponseMode = bank is HsbcBank.Sandbox
                    ? OAuth2ResponseMode.Query
                    : OAuth2ResponseMode.Fragment
            };
        }
    }
}
