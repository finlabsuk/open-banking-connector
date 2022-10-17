// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups
{
    public enum ObieBank
    {
        Modelo
    }

    //See https://openbanking.atlassian.net/wiki/spaces/DZ/pages/313918598/Integrating+a+TPP+with+Ozone+Model+Banks+Using+Postman+on+Directory+Sandbox#3.1-Dynamic-Client-Registration-(TPP)
    public class Obie : BankGroupBase<ObieBank>
    {
        protected override ConcurrentDictionary<BankProfileEnum, ObieBank> BankProfileToBank { get; } =
            new()
            {
                [BankProfileEnum.Obie_Modelo] = ObieBank.Modelo
            };

        public override BankProfile GetBankProfile(
            BankProfileEnum bankProfileEnum,
            HiddenPropertiesDictionary hiddenPropertiesDictionary)
        {
            ObieBank bank = GetBank(bankProfileEnum);
            return new BankProfile(
                bankProfileEnum,
                "https://ob19-auth1-ui.o3bank.co.uk", //from https://openbanking.atlassian.net/wiki/spaces/DZ/pages/313918598/Integrating+a+TPP+with+Ozone+Model+Banks+Using+Postman+on+Directory+Sandbox#3.1-Dynamic-Client-Registration-(TPP)
                "0015800001041RHAAY", //from https://openbanking.atlassian.net/wiki/spaces/DZ/pages/313918598/Integrating+a+TPP+with+Ozone+Model+Banks+Using+Postman+on+Directory+Sandbox#3.1-Dynamic-Client-Registration-(TPP)
                new AccountAndTransactionApi
                {
                    AccountAndTransactionApiVersion = AccountAndTransactionApiVersion.Version3p1p10,
                    BaseUrl =
                        "https://ob19-rs1.o3bank.co.uk:4501/open-banking/v3.1/aisp" // from https://openbanking.atlassian.net/wiki/spaces/DZ/pages/313918598/Integrating+a+TPP+with+the+Model+Bank+provided+by+OBIE#Accounts-End-points
                },
                new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion = PaymentInitiationApiVersion.Version3p1p6,
                    BaseUrl =
                        "https://ob19-rs1.o3bank.co.uk:4501/open-banking/v3.1/pisp" //from https://openbanking.atlassian.net/wiki/spaces/DZ/pages/313918598/Integrating+a+TPP+with+Ozone+Model+Banks+Using+Postman+on+Directory+Sandbox#3.1-Dynamic-Client-Registration-(TPP)
                },
                null,
                false)
            {
                CustomBehaviour = new CustomBehaviourClass
                {
                    BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                    {
                        ClientIdIssuedAtClaimResponseJsonConverter =
                            DateTimeOffsetConverter.UnixMilliSecondsJsonFormat
                    }
                },
                BankConfigurationApiSettings = new BankConfigurationApiSettings
                    { UseReadEndpoint = false }
            };
        }
    }
}
