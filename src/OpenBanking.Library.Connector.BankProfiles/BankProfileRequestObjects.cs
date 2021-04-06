// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles
{
    public partial class BankProfile
    {
        public BankRegistration BankRegistration(
            string name,
            Guid bankId,
            string softwareStatementProfileId,
            RegistrationScope registrationScope)
        {
            string? BankRegistrationResponsePath(RegistrationScope apiSet)
            {
                string bankRegistrationResponsesPath = AppContext.BaseDirectory; // default value
                CustomBankRegistrationResponsesPath(ref bankRegistrationResponsesPath);
                string filePath = Path.Combine(
                    bankRegistrationResponsesPath,
                    $"{RegistrationScopeApiSetHelper.AbbreviatedName(apiSet)}_{BankProfileEnum.ToString()}.json");
                return File.Exists(filePath) ? filePath : null;
            }

            return BankRegistrationAdjustments.Invoke(
                new BankRegistration
                {
                    BankId = bankId,
                    AllowMultipleRegistrations = true,
                    SoftwareStatementProfileId = softwareStatementProfileId,
                    RegistrationScope = registrationScope,
                    BankRegistrationResponseFileName = BankRegistrationResponsePath(registrationScope),
                    Name = name
                },
                registrationScope);
        }

        public Bank BankObject(string name) =>
            new Bank
            {
                IssuerUrl = IssuerUrl,
                FinancialId = FinancialId,
                Name = name
            };

        public BankApiInformation BankApiInformation(string name, Guid bankId) =>
            new BankApiInformation
            {
                BankId = bankId,
                PaymentInitiationApi = DefaultPaymentInitiationApi,
                Name = name
            };
    }
}
