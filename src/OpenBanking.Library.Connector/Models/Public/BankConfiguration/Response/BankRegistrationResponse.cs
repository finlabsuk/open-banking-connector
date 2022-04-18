// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response
{
    public interface IBankRegistrationPublicQuery : IBaseQuery
    {
        Guid BankId { get; }
    }

    /// <summary>
    ///     Response to BankRegistration ReadLocal requests.
    /// </summary>
    public class BankRegistrationReadLocalResponse : BaseResponse, IBankRegistrationPublicQuery
    {
        public BankRegistrationReadLocalResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            Guid bankId) : base(id, created, createdBy)
        {
            BankId = bankId;
        }

        public Guid BankId { get; }
    }


    /// <summary>
    ///     Response to BankRegistration Read requests
    /// </summary>
    public class BankRegistrationReadResponse : BankRegistrationReadLocalResponse
    {
        public BankRegistrationReadResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            Guid bankId,
            ClientRegistrationModelsPublic.OBClientRegistration1Response? externalApiResponse) : base(
            id,
            created,
            createdBy,
            bankId)
        {
            ExternalApiResponse = externalApiResponse;
        }


        public ClientRegistrationModelsPublic.OBClientRegistration1Response? ExternalApiResponse { get; }
    }

    /// <summary>
    ///     Response to BankRegistration Post requests
    /// </summary>
    public class BankRegistrationPostResponse : BankRegistrationReadLocalResponse
    {
        public BankRegistrationPostResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            Guid bankId,
            ClientRegistrationModelsPublic.OBClientRegistration1Response externalApiResponse) : base(
            id,
            created,
            createdBy,
            bankId)
        {
            ExternalApiResponse = externalApiResponse;
        }
        
        public ClientRegistrationModelsPublic.OBClientRegistration1Response ExternalApiResponse { get; }
    }
}
