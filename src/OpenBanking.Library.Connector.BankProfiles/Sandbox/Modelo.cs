// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.ApiModels.Base.Json;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.ClientRegistration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public static partial class BankProfileDefinitions
    {
        public static BankProfile Modelo { get; } = new BankProfile(
            BankProfileEnum.Modelo,
            "https://ob19-auth1-ui.o3bank.co.uk",
            "0015800001041RHAAY",
            ClientRegistrationApiVersion.Version3p2,
            ClientRegistrationBehaviour.CreatesNew,
            new PaymentInitiationApi
            {
                PaymentInitiationApiVersion = PaymentInitiationApiVersion.Version3p1p6,
                BaseUrl = "https://ob19-rs1.o3bank.co.uk:4501/open-banking/v3.1/pisp"
            },
            new HashSet<RegistrationScopeElement>
            {
                // Update this list based on registration or test failures
                RegistrationScopeElement.PaymentInitiation,
                RegistrationScopeElement.AccountAndTransaction,
                RegistrationScopeElement.FundsConfirmation
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
