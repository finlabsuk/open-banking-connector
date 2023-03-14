// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;

/// <summary>
///     Response to Party read requests
/// </summary>
public class PartiesResponse
{
    internal PartiesResponse(
        AccountAndTransactionModelsPublic.OBReadParty2 externalApiResponse,
        IList<string>? warnings)
    {
        ExternalApiResponse = externalApiResponse;
        Warnings = warnings;
    }

    /// <summary>
    ///     Response object OBReadParty2 from UK Open Banking Read-Write Account and Transaction API spec
    ///     <a
    ///         href="https://github.com/OpenBankingUK/read-write-api-specs/blob/v3.1.10r6/dist/openapi/account-info-openapi.yaml" />
    ///     v3.1.10 <a />. Open Banking Connector will automatically
    ///     translate <i>to</i> this from an older format for banks supporting an earlier spec version.
    /// </summary>
    public AccountAndTransactionModelsPublic.OBReadParty2 ExternalApiResponse { get; }

    /// <summary>
    ///     Optional list of warning messages from Open Banking Connector.
    /// </summary>
    public IList<string>? Warnings { get; }
}
