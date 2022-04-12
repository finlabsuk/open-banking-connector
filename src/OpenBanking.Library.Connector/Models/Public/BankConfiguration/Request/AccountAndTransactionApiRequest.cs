// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request
{
    /// <summary>
    ///     API set which specifies bank functional APIs.
    /// </summary>
    public class AccountAndTransactionApiRequest : Base, ISupportsValidation
    {
        /// <summary>
        ///     Bank with which this API is associated.
        /// </summary>
        public Guid BankId { get; set; }

        /// <summary>
        ///     Version of UK Open Banking Account and Transaction API.
        /// </summary>
        public AccountAndTransactionApiVersionEnum ApiVersion { get; set; }

        /// <summary>
        ///     Base URL for UK Open Banking Account and Transaction API.
        /// </summary>
        public string BaseUrl { get; set; } = null!;

        public async Task<ValidationResult> ValidateAsync() =>
            await new AccountAndTransactionApiRequestValidator()
                .ValidateAsync(this)!;
    }
}
