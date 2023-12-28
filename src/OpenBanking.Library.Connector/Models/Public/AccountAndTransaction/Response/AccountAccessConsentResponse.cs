// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;

public interface IAccountAccessConsentPublicQuery : IEntityBaseQuery
{
    Guid BankRegistrationId { get; }

    public string ExternalApiId { get; }
}

public abstract class AccountAccessConsentBaseResponse : ConsentBaseResponse, IAccountAccessConsentPublicQuery { }

/// <summary>
///     Response to AccountAccessConsent Create requests.
/// </summary>
public class AccountAccessConsentCreateResponse : AccountAccessConsentBaseResponse
{
    /// <summary>
    ///     Response object OBReadConsentResponse1 from UK Open Banking Read-Write Account and Transaction API spec
    ///     <a
    ///         href="https://github.com/OpenBankingUK/read-write-api-specs/blob/v3.1.10r6/dist/openapi/account-info-openapi.yaml" />
    ///     v3.1.10 <a />. Open Banking Connector will automatically
    ///     translate <i>to</i> this from an older format for banks supporting an earlier spec version.
    /// </summary>
    public AccountAndTransactionModelsPublic.OBReadConsentResponse1? ExternalApiResponse { get; init; }
}
