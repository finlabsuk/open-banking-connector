// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    public interface IBankApiInformationPublicQuery
    {
        PaymentInitiationApi? PaymentInitiationApi { get; }
        Guid Id { get; }

        Guid BankId { get; }
    }

    public class BankApiInformationResponse : IBankApiInformationPublicQuery
    {
        internal BankApiInformationResponse(PaymentInitiationApi? paymentInitiationApi, Guid id, Guid bankId)
        {
            PaymentInitiationApi = paymentInitiationApi;
            Id = id;
            BankId = bankId;
        }

        public PaymentInitiationApi? PaymentInitiationApi { get; }
        public Guid Id { get; }
        public Guid BankId { get; }
    }
}
