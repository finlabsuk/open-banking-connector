// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.RequestObjects.BankConfiguration
{
    public static class BankProfileExtensions
    {
        public static Bank GetBankRequest(this BankProfile bankProfile) =>
            new()
            {
                IssuerUrl = bankProfile.IssuerUrl,
                FinancialId = bankProfile.FinancialId,
                DynamicClientRegistrationApiVersion = bankProfile.DynamicClientRegistrationApiVersion,
                CustomBehaviour = bankProfile.CustomBehaviour,
                SupportsSca = bankProfile.SupportsSca,
                DefaultResponseMode = bankProfile.DefaultResponseMode
            };

        public static BankRegistration GetBankRegistrationRequest(
            this BankProfile bankProfile,
            Guid bankId,
            string softwareStatementProfileId,
            string? softwareStatementAndCertificateProfileOverrideCase,
            RegistrationScopeEnum? registrationScope)
        {
            var bankRegistration = new BankRegistration
            {
                BankId = bankId,
                SoftwareStatementProfileId = softwareStatementProfileId,
                SoftwareStatementAndCertificateProfileOverrideCase = softwareStatementAndCertificateProfileOverrideCase,
                RegistrationScope = registrationScope,
            };
            return bankProfile.BankConfigurationApiSettings.BankRegistrationAdjustments.Invoke(bankRegistration);
        }

        public static AccountAndTransactionApiRequest GetAccountAndTransactionApiRequest(
            this BankProfile bankProfile,
            Guid bankId) =>
            new()
            {
                BankId = bankId,
                ApiVersion =
                    bankProfile.AccountAndTransactionApi?.AccountAndTransactionApiVersion ??
                    throw new InvalidOperationException("No AccountAndTransactionApi specified in bank profile."),
                BaseUrl = bankProfile.AccountAndTransactionApi.BaseUrl,
            };

        public static PaymentInitiationApiRequest GetPaymentInitiationApiRequest(
            this BankProfile bankProfile,
            Guid bankId) =>
            new()
            {
                BankId = bankId,
                ApiVersion =
                    bankProfile.PaymentInitiationApi?.PaymentInitiationApiVersion ??
                    throw new InvalidOperationException("No PaymentInitiationApi specified in bank profile."),
                BaseUrl = bankProfile.PaymentInitiationApi.BaseUrl,
            };

        public static VariableRecurringPaymentsApiRequest GetVariableRecurringPaymentsApiRequest(
            this BankProfile bankProfile,
            Guid bankId) =>
            new()
            {
                BankId = bankId,
                ApiVersion =
                    bankProfile.VariableRecurringPaymentsApi?.VariableRecurringPaymentsApiVersion ??
                    throw new InvalidOperationException("No VariableRecurringPaymentsApi specified in bank profile."),
                BaseUrl = bankProfile.VariableRecurringPaymentsApi.BaseUrl,
            };
    }
}
