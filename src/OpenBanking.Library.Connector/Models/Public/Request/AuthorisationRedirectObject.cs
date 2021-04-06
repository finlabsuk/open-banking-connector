// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.ApiModels.Base;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;
using FluentValidation.Results;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    public class AuthorisationRedirectObject : ISupportsValidation
    {
        public AuthorisationRedirectObject(string responseMode, AuthorisationCallbackPayload response)
        {
            ResponseMode = responseMode;
            Response = response;
        }

        [JsonProperty("responseMode")]
        public string ResponseMode { get; }

        [JsonProperty("response")]
        public AuthorisationCallbackPayload Response { get; }

        public async Task<ValidationResult> ValidateAsync() =>
            await new AuthorisationRedirectObjectValidator()
                .ValidateAsync(this)!;
    }
}
