// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles
{
    public partial class BankProfile
    {
        public BankRegistration BankRegistrationRequest(
            string name,
            Guid bankId,
            string softwareStatementProfileId,
            string? softwareStatementAndCertificateProfileOverrideCase,
            RegistrationScopeEnum registrationScope,
            string? openIdConfigurationReplacement)
        {
            var bankRegistration = new BankRegistration
            {
                Name = name,
                BankId = bankId,
                SoftwareStatementProfileId = softwareStatementProfileId,
                SoftwareStatementAndCertificateProfileOverrideCase = softwareStatementAndCertificateProfileOverrideCase,
                RegistrationScope = registrationScope,
                ClientRegistrationApi = ClientRegistrationApiVersion,
                AllowMultipleRegistrations = true,
                OpenIdConfigurationReplacement = openIdConfigurationReplacement
            };
            return ClientRegistrationApiSettings.BankRegistrationAdjustments.Invoke(
                bankRegistration,
                registrationScope);
        }

        public Bank BankRequest(string name) =>
            new()
            {
                IssuerUrl = IssuerUrl,
                FinancialId = FinancialId,
                Name = name
            };

        public AccountAndTransactionApiRequest GetAccountAndTransactionApiRequest(Guid bankId) =>
            new()
            {
                BankId = bankId,
                ApiVersion =
                    AccountAndTransactionApi?.AccountAndTransactionApiVersion ??
                    throw new InvalidOperationException(),
                BaseUrl = AccountAndTransactionApi.BaseUrl,
            };

        public PaymentInitiationApiRequest GetPaymentInitiationApiRequest(Guid bankId) =>
            new()
            {
                BankId = bankId,
                ApiVersion =
                    PaymentInitiationApi?.PaymentInitiationApiVersion ??
                    throw new InvalidOperationException(),
                BaseUrl = PaymentInitiationApi.BaseUrl,
            };

        public VariableRecurringPaymentsApiRequest GetVariableRecurringPaymentsApiRequest(Guid bankId) =>
            new()
            {
                BankId = bankId,
                ApiVersion =
                    VariableRecurringPaymentsApi?.VariableRecurringPaymentsApiVersion ??
                    throw new InvalidOperationException(),
                BaseUrl = VariableRecurringPaymentsApi.BaseUrl,
            };
    }
}
