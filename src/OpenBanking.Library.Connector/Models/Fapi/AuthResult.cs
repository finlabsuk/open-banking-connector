// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.ExternalApiBase;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;
using FluentValidation.Results;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi
{
    /// <summary>
    ///     Auth response supplied by redirect (or according to response_mode).
    ///     We call this <see cref="AuthResult" /> to avoid using "response" word in Fluent request class.
    /// </summary>
    public class AuthResult : ISupportsValidation
    {
        public AuthResult(string idToken, string code, string state, string? nonce)
        {
            IdToken = idToken;
            Code = code;
            State = state;
            Nonce = nonce;
        }


        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("nonce", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? Nonce { get; set; }

        public async Task<ValidationResult> ValidateAsync() =>
            await new AuthResultValidator()
                .ValidateAsync(this)!;
    }
}
