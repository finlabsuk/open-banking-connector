﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using Newtonsoft.Json;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request
{
    /// <summary>
    ///     UK Open Banking Account and Transaction functional API.
    /// </summary>
    public class AccountAndTransactionApiRequest : Base, ISupportsValidation
    {
        /// <summary>
        ///     BankProfile used to supply default values for unspecified properties and apply transformations to external API
        ///     requests.
        /// </summary>
        public BankProfileEnum? BankProfile { get; set; }

        /// <summary>
        ///     Bank with which this API is associated.
        /// </summary>
        [Required]
        [JsonProperty(Required = Required.Always)]
        public Guid BankId { get; set; }

        /// <summary>
        ///     Version of UK Open Banking Account and Transaction API.
        ///     When null, value obtained from BankProfile.
        /// </summary>
        public AccountAndTransactionApiVersion? ApiVersion { get; set; }

        /// <summary>
        ///     Base URL for UK Open Banking Account and Transaction API.
        ///     When null, value obtained from BankProfile.
        /// </summary>
        public string? BaseUrl { get; set; }

        public async Task<ValidationResult> ValidateAsync() =>
            await new AccountAndTransactionApiRequestValidator()
                .ValidateAsync(this)!;
    }
}
