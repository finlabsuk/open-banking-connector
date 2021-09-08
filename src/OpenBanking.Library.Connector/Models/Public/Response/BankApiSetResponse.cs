// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    public interface IBankApiSetPublicQuery : IBaseQuery
    {
        PaymentInitiationApi? PaymentInitiationApi { get; }

        Guid BankId { get; }
    }

    /// <summary>
    ///     Response to GetLocal
    /// </summary>
    public class BankApiSetResponse : BaseResponse, IBankApiSetPublicQuery
    {
        internal BankApiSetResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            PaymentInitiationApi? paymentInitiationApi,
            Guid bankId) : base(id, name, created, createdBy)
        {
            PaymentInitiationApi = paymentInitiationApi;
            BankId = bankId;
        }

        public PaymentInitiationApi? PaymentInitiationApi { get; }
        public Guid BankId { get; }
    }
}
