// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation
{
    public class ApiProfile : IEntity
    {
        public ApiProfile(
            string id,
            string bankClientProfileId,
            ApiVersion apiVersion,
            string baseUrl)
        {
            Id = id;
            BankClientProfileId = bankClientProfileId;
            ApiVersion = apiVersion;
            BaseUrl = baseUrl;
        }

        public string BankClientProfileId { get; }

        public ApiVersion ApiVersion { get; }

        public string BaseUrl { get; }

        public string Id { get; }
    }
}
