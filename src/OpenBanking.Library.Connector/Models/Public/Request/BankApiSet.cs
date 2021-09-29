// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    /// <summary>
    ///     API set which specifies bank functional APIs.
    /// </summary>
    public class BankApiSet : Base, ISupportsValidation
    {
        /// <summary>
        ///     Bank with which this API set is associated.
        /// </summary>
        public Guid BankId { get; set; }

        /// <summary>
        ///     Specifies UK Open Banking Payment Initiation API.
        ///     Null means no such API in this API set.
        /// </summary>
        public PaymentInitiationApi? PaymentInitiationApi { get; set; }

        /// <summary>
        ///     Specifies UK Open Banking Variable Recurring Payments API.
        ///     Null means no such API in this API set.
        /// </summary>
        public VariableRecurringPaymentsApi? VariableRecurringPaymentsApi { get; set; }

        public async Task<ValidationResult> ValidateAsync() =>
            await new BankApiSetValidator()
                .ValidateAsync(this)!;
    }
}
