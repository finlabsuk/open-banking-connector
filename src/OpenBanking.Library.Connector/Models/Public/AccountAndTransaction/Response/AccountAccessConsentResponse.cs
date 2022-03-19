// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using AccountAndTransactionModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response
{
    public interface IAccountAccessConsentPublicQuery : IBaseQuery
    {
        ReadWriteProperty<AccountAndTransactionModelsPublic.OBReadConsentResponse1> OBReadConsentResponse { get; }

        Guid BankRegistrationId { get; }

        Guid BankApiSetId { get; }
    }

    public class AccountAccessConsentResponse : BaseResponse, IAccountAccessConsentPublicQuery
    {
        public AccountAccessConsentResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            ReadWriteProperty<AccountAndTransactionModelsPublic.OBReadConsentResponse1> obReadConsentResponse,
            Guid bankRegistrationId,
            Guid bankApiSetId) : base(id, name, created, createdBy)
        {
            OBReadConsentResponse = obReadConsentResponse;
            BankRegistrationId = bankRegistrationId;
            BankApiSetId = bankApiSetId;
        }

        /// <summary>
        ///     Response object from UK Open Banking spec v3.1.9. Open Banking Connector can be configured
        ///     to translate to this for banks supporting an earlier spec version.
        /// </summary>
        public ReadWriteProperty<AccountAndTransactionModelsPublic.OBReadConsentResponse1> OBReadConsentResponse
        {
            get;
        }

        public Guid BankRegistrationId { get; }
        public Guid BankApiSetId { get; }
    }
}
