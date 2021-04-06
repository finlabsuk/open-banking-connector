// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.ApiModels.Base;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request
{
    public class DomesticPaymentConsentAuthContext : ISupportsValidation
    {
        public Guid DomesticPaymentConsentId { get; set; }

        /// <summary>
        ///     Friendly name to support debugging etc. (must be unique i.e. not already in use).
        ///     This is optional.
        /// </summary>
        public string? Name { get; set; }

        public async Task<ValidationResult> ValidateAsync() =>
            await new AuthContextValidator()
                .ValidateAsync(this)!;
    }
}
