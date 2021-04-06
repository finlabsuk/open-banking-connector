// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public
{
    /// <summary>
    ///     UK Open Banking API configured in bank registration scope
    /// </summary>
    public enum RegistrationScopeElement
    {
        [EnumMember(Value = "accountAndTransaction")]
        AccountAndTransaction,

        [EnumMember(Value = "paymentInitiation")]
        PaymentInitiation,

        [EnumMember(Value = "fundsConfirmation")]
        FundsConfirmation,
    }

    public static class RegistrationScopeApiHelper
    {
        static RegistrationScopeApiHelper()
        {
            AllApiTypes = Enum.GetValues(typeof(RegistrationScopeElement))
                .Cast<RegistrationScopeElement>();
        }

        public static IEnumerable<RegistrationScopeElement> AllApiTypes { get; }

        public static RegistrationScope ApiTypeSetWithSingleApiType(
            RegistrationScopeElement registrationScopeElement) =>
            registrationScopeElement switch
            {
                RegistrationScopeElement.AccountAndTransaction => RegistrationScope.AccountAndTransaction,
                RegistrationScopeElement.PaymentInitiation => RegistrationScope.PaymentInitiation,
                RegistrationScopeElement.FundsConfirmation => RegistrationScope.FundsConfirmation,
                _ => throw new ArgumentException(
                    $"{nameof(registrationScopeElement)} is not valid ApiType or needs to be added to this switch statement.")
            };

        /// <summary>
        ///     Mapping from individual API type to scope word
        /// </summary>
        public static string ScopeWord(RegistrationScopeElement registrationScopeElement) =>
            registrationScopeElement switch
            {
                RegistrationScopeElement.AccountAndTransaction => "accounts",
                RegistrationScopeElement.PaymentInitiation => "payments",
                RegistrationScopeElement.FundsConfirmation => "fundsconfirmations",
                _ => throw new ArgumentException(
                    $"{nameof(registrationScopeElement)} is not valid ApiType or needs to be added to this switch statement.")
            };
    }
}
