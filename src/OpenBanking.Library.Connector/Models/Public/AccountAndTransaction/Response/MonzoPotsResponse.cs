﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;

/// <summary>
///     Response to Monzo pot read requests
/// </summary>
public class MonzoPotsResponse
{
    internal MonzoPotsResponse(
        ReadMonzoPot externalApiResponse,
        IList<string>? warnings)
    {
        ExternalApiResponse = externalApiResponse;
        Warnings = warnings;
    }

    public ReadMonzoPot ExternalApiResponse { get; }

    /// <summary>
    ///     Optional list of warning messages from Open Banking Connector.
    /// </summary>
    public IList<string>? Warnings { get; }
}