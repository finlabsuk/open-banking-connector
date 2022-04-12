// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response
{
    public interface IAccountAndTransactionApiQuery : IBaseQuery
    {
        AccountAndTransactionApiVersionEnum ApiVersion { get; }

        string BaseUrl { get; }
        Guid BankId { get; }
    }

    /// <summary>
    ///     Response to GetLocal
    /// </summary>
    public class AccountAndTransactionApiResponse : BaseResponse, IAccountAndTransactionApiQuery
    {
        public AccountAndTransactionApiResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            AccountAndTransactionApiVersionEnum apiVersion,
            string baseUrl,
            Guid bankId) : base(id, name, created, createdBy)
        {
            ApiVersion = apiVersion;
            BaseUrl = baseUrl;
            BankId = bankId;
        }

        /// <summary>
        ///     Bank with which this API is associated.
        /// </summary>
        public Guid BankId { get; }

        /// <summary>
        ///     Version of UK Open Banking Account and Transaction API.
        /// </summary>
        public AccountAndTransactionApiVersionEnum ApiVersion { get; set; }

        /// <summary>
        ///     Base URL for UK Open Banking Account and Transaction API.
        /// </summary>
        public string BaseUrl { get; set; }
    }
}
