// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent.PaymentInitiation
{
    public class ApiProfile
    {
        public ApiProfile(string id, string bankClientProfileId, Public.PaymentInitiation.ApiVersion apiVersion,
            string baseUrl)
        {
            Id = id;
            BankClientProfileId = bankClientProfileId;
            ApiVersion = apiVersion;
            BaseUrl = baseUrl;
        }

        public string Id { get; set; }

        public string BankClientProfileId { get; }

        public Public.PaymentInitiation.ApiVersion ApiVersion { get; }

        public string BaseUrl { get; }
    }
}
