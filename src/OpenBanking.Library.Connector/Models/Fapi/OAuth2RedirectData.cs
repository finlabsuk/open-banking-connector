// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;
using FluentValidation.Results;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi
{
    /// <summary>
    ///     Auth response supplied by redirect according to response_mode.
    ///     We call this <see cref="OAuth2RedirectData" /> to avoid using "response" word in Fluent request class.
    /// </summary>
    public class OAuth2RedirectData : ISupportsValidation
    {
        public OAuth2RedirectData(
            string idToken,
            string code,
            string state)
        {
            IdToken = idToken;
            Code = code;
            State = state;
        }

        /// <summary>
        ///     Required data in redirect when "response_type" = "code id_token"
        /// </summary>
        [JsonProperty("id_token", Required = Required.Always)]
        public string IdToken { get; set; }

        /// <summary>
        ///     Required data in redirect when "response_type" = "code id_token"
        /// </summary>
        [JsonProperty("code", Required = Required.Always)]
        public string Code { get; set; }

        /// <summary>
        ///     Required data in redirect when "response_type" = "code id_token" and "state" is request parameter
        /// </summary>
        [JsonProperty("state", Required = Required.Always)]
        public string State { get; set; }

        public async Task<ValidationResult> ValidateAsync() =>
            await new AuthResultValidator()
                .ValidateAsync(this)!;
    }
}
