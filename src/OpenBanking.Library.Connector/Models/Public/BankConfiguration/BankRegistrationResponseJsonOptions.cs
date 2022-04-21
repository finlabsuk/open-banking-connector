﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration
{
    /// <summary>
    ///     Class used to specify JSON options for returned registration response
    ///     from bank API.
    ///     Sometimes bank responses need custom JSON de-serialisation
    ///     and this class supports this.
    ///     Default (e.g. null) property values do not lead to changes.
    /// </summary>
    public class BankRegistrationResponseJsonOptions
    {
        public DateTimeOffsetToUnixConverterOptions ClientIdIssuedAtConverterOptions { get; set; } =
            DateTimeOffsetToUnixConverterOptions.None;

        public DelimitedStringConverterOptions ScopeConverterOptions { get; set; } =
            DelimitedStringConverterOptions.None;
    }
}