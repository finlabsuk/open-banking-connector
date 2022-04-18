// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response
{
    public interface IBankPublicQuery : IBaseQuery
    {
        string IssuerUrl { get; }
        string FinancialId { get; }
    }

    /// <summary>
    ///     Response to GetLocal
    /// </summary>
    public class BankResponse : BaseResponse, IBankPublicQuery
    {
        internal BankResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string issuerUrl,
            string financialId) : base(id, created, createdBy)
        {
            IssuerUrl = issuerUrl;
            FinancialId = financialId;
        }

        public string IssuerUrl { get; }

        public string FinancialId { get; }
    }
}
