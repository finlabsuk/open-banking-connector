// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public partial class BankProfileDefinitions
    {
        public BankProfile VrpHackathon { get; }

        private BankProfile GetVrpHackathon()
        {
            BankProfileHiddenProperties bankProfileHiddenProperties =
                GetRequiredBankProfileHiddenProperties(BankProfileEnum.VrpHackathon);
            return new BankProfile(
                BankProfileEnum.VrpHackathon,
                bankProfileHiddenProperties.GetRequiredIssuerUrl(),
                bankProfileHiddenProperties.GetRequiredFinancialId(),
                bankProfileHiddenProperties.GetRequiredClientRegistrationApiVersion(),
                null,
                null,
                new VariableRecurringPaymentsApi
                {
                    VariableRecurringPaymentsApiVersion = bankProfileHiddenProperties
                        .GetRequiredVariableRecurringPaymentsApiVersion(),
                    BaseUrl = bankProfileHiddenProperties
                        .GetRequiredVariableRecurringPaymentsApiBaseUrl()
                });
        }
    }
}
