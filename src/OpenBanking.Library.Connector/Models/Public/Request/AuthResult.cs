﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;
using FluentValidation.Results;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    public class AuthResult : ISupportsValidation
    {
        public AuthResult(OAuth2ResponseMode responseMode, OAuth2RedirectData redirectData)
        {
            ResponseMode = responseMode;
            RedirectData = redirectData;
        }

        /// <summary>
        ///     OpenID Connect response_mode used by redirect that supplied this result. This is
        ///     used to check result came by way of correct response_mode as security aspects
        ///     of response_modes differ.
        /// </summary>
        [JsonProperty("response_mode")]
        public OAuth2ResponseMode ResponseMode { get; }

        public OAuth2RedirectData RedirectData { get; }

        public async Task<ValidationResult> ValidateAsync() =>
            await new AuthorisationRedirectObjectValidator()
                .ValidateAsync(this)!;
    }
}