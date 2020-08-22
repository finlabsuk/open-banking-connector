// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    public class BankProfileResponse
    {
        internal BankProfileResponse(BankProfile bankProfile)
        {
            BankRegistrationId = bankProfile.BankRegistrationId;
            PaymentInitiationApi = bankProfile.PaymentInitiationApi;
            Id = bankProfile.Id;
        }

        public string BankRegistrationId { get; }
        public PaymentInitiationApi? PaymentInitiationApi { get; }
        public string Id { get; }
    }
}
