// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    /// <summary>
    ///     A Bank is the base type used to describe a bank ("ASPSP") in OBC.
    ///     It effectively captures the static data associated with a bank which is then used when
    ///     creating bank registrations and profiles for that bank.
    ///     Each <see cref="BankRegistration" /> and <see cref="BankProfile" /> is
    ///     a child object of a Bank.
    ///     Such child objects can, when created, update their Bank parent to point to them
    ///     as e.g. the "default bank registration" or "default bank profile" allowing
    ///     the Bank name to be used as a shorthand instead of the child object ID in
    ///     various API calls.
    /// </summary>
    public class Bank
    {
        /// <summary>
        ///     Functional APIs used for bank registration
        /// </summary>
        public RegistrationScopeApiSet RegistrationScopeApiSet { get; set; }

        /// <summary>
        ///     Issuer URL to use when creating Bank Registration
        /// </summary>
        public string IssuerUrl { get; set; } = null!;

        /// <summary>
        ///     FAPI financial ID to use when creating Bank Registration
        /// </summary>
        public string FinancialId { get; set; } = null!;

        /// <summary>
        ///     Name to use for bank (must be unique i.e. not already in use).
        /// </summary>
        public string Name { get; set; } = null!;
    }
}
