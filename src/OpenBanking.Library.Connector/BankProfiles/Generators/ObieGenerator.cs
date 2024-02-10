// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

//See https://openbanking.atlassian.net/wiki/spaces/DZ/pages/313918598/Integrating+a+TPP+with+Ozone+Model+Banks+Using+Postman+on+Directory+Sandbox#3.1-Dynamic-Client-Registration-(TPP)
public class ObieGenerator : BankProfileGeneratorBase<ObieBank>
{
    public ObieGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider,
        IBankGroup<ObieBank> bankGroup) : base(bankProfilesSettingsProvider, bankGroup) { }

    public override BankProfile GetBankProfile(ObieBank bank)
    {
        return new BankProfile(
            _bankGroup.GetBankProfile(bank),
            bank switch
            {
                ObieBank.Modelo =>
                    "https://ob19-auth1-ui.o3bank.co.uk", //from https://openbanking.atlassian.net/wiki/spaces/DZ/pages/313918598/Integrating+a+TPP+with+Ozone+Model+Banks+Using+Postman+on+Directory+Sandbox#3.1-Dynamic-Client-Registration-(TPP)
                ObieBank.Model2023 =>
                    "https://auth1.obie.uk.ozoneapi.io", // from https://github.com/OpenBankingUK/OBL-ModelBank-Integration
                _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
            },
            "0015800001041RHAAY", // from https://github.com/OpenBankingUK/OBL-ModelBank-Integration
            new AccountAndTransactionApi
            {
                BaseUrl = bank switch
                {
                    ObieBank.Modelo =>
                        "https://ob19-rs1.o3bank.co.uk:4501/open-banking/v3.1/aisp", // from https://openbanking.atlassian.net/wiki/spaces/DZ/pages/313918598/Integrating+a+TPP+with+the+Model+Bank+provided+by+OBIE#Accounts-End-points
                    ObieBank.Model2023 =>
                        "https://rs1.obie.uk.ozoneapi.io/open-banking/v3.1/aisp", // from https://github.com/OpenBankingUK/OBL-ModelBank-Integration
                    _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
                }
            },
            new PaymentInitiationApi
            {
                BaseUrl = bank switch
                {
                    ObieBank.Modelo =>
                        "https://ob19-rs1.o3bank.co.uk:4501/open-banking/v3.1/pisp", //from https://openbanking.atlassian.net/wiki/spaces/DZ/pages/313918598/Integrating+a+TPP+with+Ozone+Model+Banks+Using+Postman+on+Directory+Sandbox#3.1-Dynamic-Client-Registration-(TPP)
                    ObieBank.Model2023 =>
                        "https://rs1.obie.uk.ozoneapi.io/open-banking/v3.1/pisp", // from https://github.com/OpenBankingUK/OBL-ModelBank-Integration
                    _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
                }
            },
            null,
            false)
        {
            CustomBehaviour = bank is ObieBank.Modelo
                ? new CustomBehaviourClass
                {
                    BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                    {
                        TransportCertificateSubjectDnOrgIdEncoding = SubjectDnOrgIdEncoding.DottedDecimalAttributeType,
                        ClientIdIssuedAtClaimResponseJsonConverter =
                            DateTimeOffsetUnixConverterEnum.UnixMilliSecondsJsonFormat
                    },
                    AuthCodeGrantPost = new AuthCodeAndRefreshTokenGrantPostCustomBehaviour
                    {
                        AllowNullResponseRefreshToken = true // required for PISP case
                    }
                }
                : new CustomBehaviourClass
                {
                    BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                    {
                        ClientIdIssuedAtClaimResponseJsonConverter =
                            DateTimeOffsetUnixConverterEnum.UnixMilliSecondsJsonFormat
                    }
                },
            BankConfigurationApiSettings = new BankConfigurationApiSettings
            {
                UseRegistrationDeleteEndpoint = true,
                TestTemporaryBankRegistration = bank is ObieBank.Modelo
            }
        };
    }
}
