﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response
{
    public interface IAccountAndTransactionApiQuery : IBaseQuery
    {
        Guid BankId { get; }
        AccountAndTransactionApiVersion ApiVersion { get; }

        string BaseUrl { get; }
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
            Guid bankId,
            AccountAndTransactionApiVersion apiVersion,
            string baseUrl) : base(id, name, created, createdBy)
        {
            BankId = bankId;
            ApiVersion = apiVersion;
            BaseUrl = baseUrl;
        }

        /// <summary>
        ///     Bank with which this API is associated.
        /// </summary>
        public Guid BankId { get; }

        /// <summary>
        ///     Version of UK Open Banking Account and Transaction API.
        /// </summary>
        public AccountAndTransactionApiVersion ApiVersion { get; set; }

        /// <summary>
        ///     Base URL for UK Open Banking Account and Transaction API.
        /// </summary>
        public string BaseUrl { get; set; }
    }
}