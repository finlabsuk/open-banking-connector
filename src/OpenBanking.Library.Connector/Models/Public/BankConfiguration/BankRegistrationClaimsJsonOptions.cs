// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration
{
    /// <summary>
    ///     Class used to specify JSON options for registration request
    ///     to bank API.
    ///     Sometimes bank requests need custom JSON serialisation
    ///     and this class supports this.
    ///     Default (e.g. null) property values do not lead to changes.
    /// </summary>
    public class BankRegistrationClaimsJsonOptions
    {
        public DelimitedStringConverterOptions ScopeConverterOptions { get; set; } =
            DelimitedStringConverterOptions.None;
    }
}
