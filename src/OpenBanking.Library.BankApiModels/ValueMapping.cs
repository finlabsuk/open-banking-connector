// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

namespace FinnovationLabs.OpenBanking.Library.BankApiModels
{
    /// <summary>
    ///     Type of value mapping. Value mappings are used to customise AutoMapper-based mapping of types.
    /// </summary>
    public enum ValueMapping
    {
        SetNull,
        CommaDelimitedStringToIEnumerable,
        CommaDelimitedStringToIEnumerableReverse,
        StringIdentityValueConverter
    }
}
