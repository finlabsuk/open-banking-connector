// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles
{
    public partial class BankProfile
    {
        public BankRegistration BankRegistration(
            Guid bankId,
            string softwareStatementProfileId,
            RegistrationScopeApiSet registrationScopeApiSet)
        {
            string? BankRegistrationResponsePath(RegistrationScopeApiSet apiSet)
            {
                string bankRegistrationResponsesPath = AppContext.BaseDirectory; // default value
                CustomBankRegistrationResponsesPath(ref bankRegistrationResponsesPath);
                string filePath = Path.Combine(
                    path1: bankRegistrationResponsesPath,
                    path2:
                    $"{RegistrationScopeApiSetHelper.AbbreviatedName(apiSet)}_{BankProfileEnum.ToString()}.json");
                return File.Exists(filePath) ? filePath : null;
            }

            return BankRegistrationAdjustments.Invoke(
                bankRegistration: new BankRegistration
                {
                    BankId = bankId,
                    AllowMultipleRegistrations = true,
                    ReplaceStagingBankRegistration = true,
                    SoftwareStatementProfileId = softwareStatementProfileId,
                    BankRegistrationResponseFileName = BankRegistrationResponsePath(registrationScopeApiSet)
                },
                registrationScopeApiSet: registrationScopeApiSet);
        }

        public Bank BankObject(
            string name,
            RegistrationScopeApiSet registrationScopeApiSet) =>
            new Bank
            {
                RegistrationScopeApiSet = registrationScopeApiSet,
                IssuerUrl = IssuerUrl,
                FinancialId = FinancialId,
                Name = name
            };

        public BankApiInformation BankApiInformation(Guid bankId) =>
            new BankApiInformation
            {
                BankId = bankId,
                ReplaceStagingBankProfile = true,
                PaymentInitiationApi = DefaultPaymentInitiationApi
            };
    }
}
