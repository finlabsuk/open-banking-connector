// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public
{
    /// <summary>
    ///     Registration scope used when creating a bank client.
    ///     Set of <see cref="RegistrationScopeElement" />.
    /// </summary>
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RegistrationScope
    {
        [EnumMember(Value = "none")] None = 0,

        [EnumMember(Value = "accountAndTransaction")]
        AccountAndTransaction = 1,

        [EnumMember(Value = "paymentInitiation")]
        PaymentInitiation = 2,

        [EnumMember(Value = "fundsConfirmation")]
        FundsConfirmation = 4,

        [EnumMember(Value = "all")] All = AccountAndTransaction | PaymentInitiation | FundsConfirmation,
    }

    public static class RegistrationScopeExtensions
    {
        /// <summary>
        ///     Abbreviated name for registration scope
        /// </summary>
        /// <param name="registrationScope"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string AbbreviatedName(this RegistrationScope registrationScope)
        {
            return registrationScope switch
            {
                // RegistrationScope.None: invalid
                RegistrationScope.AccountAndTransaction => "AT",
                RegistrationScope.PaymentInitiation => "PI",
                RegistrationScope.FundsConfirmation => "FC",
                RegistrationScope.All => "All",
                _ => throw new ArgumentOutOfRangeException(nameof(registrationScope), registrationScope, null)
            };
        }
    }
}
