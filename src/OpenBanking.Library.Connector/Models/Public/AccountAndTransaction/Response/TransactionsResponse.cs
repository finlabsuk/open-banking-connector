// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;

/// <summary>
///     Response to Transaction read requests
/// </summary>
public class TransactionsResponse : BaseResponse
{
    /// <summary>
    ///     Response object OBReadTransaction6 from UK Open Banking Read-Write Account and Transaction API spec
    ///     <a
    ///         href="https://github.com/OpenBankingUK/read-write-api-specs/blob/v3.1.10r6/dist/openapi/account-info-openapi.yaml" />
    ///     v3.1.10 <a />. Open Banking Connector will automatically
    ///     translate <i>to</i> this from an older format for banks supporting an earlier spec version.
    /// </summary>
    public required AccountAndTransactionModelsPublic.OBReadTransaction6 ExternalApiResponse { get; init; }

    /// <summary>
    ///     Additional info relating to response from external (bank) API.
    /// </summary>
    public required ExternalApiResponseInfo ExternalApiResponseInfo { get; init; }
}
