// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.Json
{
    /// <summary>
    ///     Label for particular usages of a JSON converter. This allows bank-specific
    ///     options to be used for these usages to e.g. correct serialisation errors. If a label is not both specified AND
    ///     activated via serialiser settings,
    ///     the default version of a JSON converter (which may be a "pass-through") is always used.
    /// </summary>
    public enum JsonConverterLabel
    {
        DcrRegClientIdIssuedAt,
        DcrRegScope,
        IdTokenExpirationTimeClaim,
        DirectDebitPreviousPaymentDateTime
    }
}
