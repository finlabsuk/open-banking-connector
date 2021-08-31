// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.Json
{
    /// <summary>
    /// Label for particular usages of a JSON converter. This allows bank-specific
    /// options to be used for these usages to e.g. correct serialisation errors.
    /// </summary>
    public enum JsonConverterLabel
    {
        /// <summary>
        /// No label is the default label
        /// </summary>
        NoLabel,
        DcrRegClientIdIssuedAt,
        DcrRegScope
    }
}
