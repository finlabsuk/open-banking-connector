// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    public interface IBankProfilePublicQuery
    {
        string BankRegistrationId { get; }
        PaymentInitiationApi? PaymentInitiationApi { get; }
        string Id { get; }
    }

    public class BankProfileResponse : IBankProfilePublicQuery
    {
        internal BankProfileResponse(string bankRegistrationId, PaymentInitiationApi? paymentInitiationApi, string id)
        {
            BankRegistrationId = bankRegistrationId;
            PaymentInitiationApi = paymentInitiationApi;
            Id = id;
        }

        public string BankRegistrationId { get; }
        public PaymentInitiationApi? PaymentInitiationApi { get; }
        public string Id { get; }
    }
}
