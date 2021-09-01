// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles
{
    public partial class BankProfile
    {
        public BankRegistration BankRegistrationRequest(
            string name,
            Guid bankId,
            string softwareStatementProfileId,
            RegistrationScope registrationScope)
        {
            var bankRegistration = new BankRegistration
            {
                Name = name,
                BankId = bankId,
                SoftwareStatementProfileId = softwareStatementProfileId,
                RegistrationScope = registrationScope,
                ClientRegistrationApi = ClientRegistrationApiVersion,
                AllowMultipleRegistrations = true
            };
            return ClientRegistrationApiSettings.BankRegistrationAdjustments.Invoke(
                bankRegistration,
                registrationScope);
        }

        public Bank BankRequest(string name) =>
            new Bank
            {
                IssuerUrl = IssuerUrl,
                FinancialId = FinancialId,
                Name = name
            };

        public BankApiInformation BankApiInformationRequest(string name, Guid bankId) =>
            new BankApiInformation
            {
                BankId = bankId,
                PaymentInitiationApi = PaymentInitiationApi,
                Name = name
            };
    }
}
