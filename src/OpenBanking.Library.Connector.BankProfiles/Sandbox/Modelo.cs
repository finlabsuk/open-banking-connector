// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.ObApi.Base.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public static partial class BankProfileDefinitions
    {
        public static BankProfile Modelo { get; } = new BankProfile(
            bankProfileEnum: BankProfileEnum.Modelo,
            issuerUrl: "https://ob19-auth1-ui.o3bank.co.uk",
            financialId: "0015800001041RHAAY",
            clientRegistrationBehaviour: ClientRegistrationBehaviour.CreatesNew,
            defaultPaymentInitiationApi: new PaymentInitiationApi
            {
                ApiVersion = ApiVersion.V3p1p4,
                BaseUrl = "https://ob19-rs1.o3bank.co.uk:4501/open-banking/v3.1/pisp"
            },
            supportedApisInRegistrationScope: new HashSet<RegistrationScopeApi>
            {
                // Update this list based on registration or test failures
                RegistrationScopeApi.PaymentInitiation,
                RegistrationScopeApi.AccountAndTransaction,
                RegistrationScopeApi.FundsConfirmation
            })
        {
            BankRegistrationAdjustments = (registration, set) =>
            {
                registration.BankRegistrationResponseJsonOptions = new BankRegistrationResponseJsonOptions
                {
                    DateTimeOffsetUnixConverterOptions = DateTimeOffsetUnixConverterOptions.MilliSecondsNotSeconds
                };
                return registration;
            },
        };
    }
}
