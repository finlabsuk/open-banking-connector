// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

public class BaseResponse
{
    /// <summary>
    ///     Optional list of warning messages from Open Banking Connector.
    /// </summary>
    public IList<string>? Warnings { get; init; }
}
