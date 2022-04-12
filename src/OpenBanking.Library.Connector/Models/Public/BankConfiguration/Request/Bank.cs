// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request
{
    /// <summary>
    ///     A Bank is the base type used to describe a bank ("ASPSP") in OBC.
    ///     It effectively captures the static data associated with a bank which is then used when
    ///     creating bank registrations and profiles for that bank.
    ///     Each <see cref="BankRegistration" /> and <see cref="BankApiSet" /> is
    ///     a child object of a Bank.
    ///     Such child objects can, when created, update their Bank parent to point to them
    ///     as e.g. the "default bank registration" or "default bank profile" allowing
    ///     the Bank name to be used as a shorthand instead of the child object ID in
    ///     various API calls.
    /// </summary>
    public class Bank : Base, ISupportsValidation
    {
        /// <summary>
        ///     Issuer URL to use when creating Bank Registration
        /// </summary>
        public string IssuerUrl { get; set; } = null!;

        /// <summary>
        ///     FAPI financial ID to use when creating Bank Registration
        /// </summary>
        public string FinancialId { get; set; } = null!;

        public async Task<ValidationResult> ValidateAsync() =>
            await new BankValidator()
                .ValidateAsync(this)!;
    }
}
