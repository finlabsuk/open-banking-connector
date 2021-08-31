// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    public interface IBankRegistrationPublicQuery : IBaseQuery
    {
        ReadWriteProperty<ClientRegistrationModelsPublic.OBClientRegistration1Response> BankApiResponse { get; }

        Guid BankId { get; }
    }

    /// <summary>
    ///     Respnose to GetLocal
    /// </summary>
    public class BankRegistrationResponse : BaseResponse, IBankRegistrationPublicQuery
    {
        public BankRegistrationResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            ReadWriteProperty<ClientRegistrationModelsPublic.OBClientRegistration1Response> bankApiResponse,
            Guid bankId) : base(id, name, created, createdBy)
        {
            BankApiResponse = bankApiResponse;
            BankId = bankId;
        }

        public ReadWriteProperty<ClientRegistrationModelsPublic.OBClientRegistration1Response> BankApiResponse { get; }
        public Guid BankId { get; }
    }
}
