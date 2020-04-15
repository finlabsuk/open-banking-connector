// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    public class BankClientProfileResponse
    {
        public BankClientProfileResponse(BankClientProfile  persistentProfile)
        {
            Id = persistentProfile.Id;
            BankClientRegistrationClaims = persistentProfile.BankClientRegistrationClaims;
        }        

        /// ID used to uniquely identify object
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("bankClientRegistrationClaims")]
        public BankClientRegistrationClaims BankClientRegistrationClaims { get; set; }
    }
}
