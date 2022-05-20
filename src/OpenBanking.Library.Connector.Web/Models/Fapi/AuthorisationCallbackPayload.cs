﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Fapi
{
    
    public class AuthorisationCallbackPayloadQuery
    {
        /// <summary>
        ///     Required response when "response_type" = "code id_token"
        /// </summary>
        [FromQuery(Name = "id_token")]
        public string IdToken { get; set; } = null!;

        /// <summary>
        ///     Required response when "response_type" = "code id_token"
        /// </summary>
        [FromQuery(Name = "code")]
        public string Code { get; set; } = null!;

        /// <summary>
        ///     Required response when "response_type" = "code id_token" and "state" is request parameter
        /// </summary>
        [FromQuery(Name = "state")]
        public string State { get; set; } = null!;

        [FromQuery(Name = "nonce")]
        public string? Nonce { get; set; }
    }

    public static class AuthorisationCallbackPayloadExtensions
    {
        public static AuthResult ToLibraryVersion(this AuthorisationCallbackPayloadQuery payload) =>
            new(
                payload.IdToken,
                payload.Code,
                payload.State,
                payload.Nonce);

        
        public static AuthResult ToLibraryVersion(this AuthorisationCallbackPayload payload) =>
            new(
                payload.IdToken,
                payload.Code,
                payload.State,
                payload.Nonce);
    }

    public class AuthorisationCallbackPayload
    {
        /// <summary>
        ///     Required response when "response_type" = "code id_token"
        /// </summary>
        [FromForm(Name = "id_token")]
        public string IdToken { get; set; } = null!;

        /// <summary>
        ///     Required response when "response_type" = "code id_token"
        /// </summary>
        [FromForm(Name = "code")]
        public string Code { get; set; } = null!;

        /// <summary>
        ///     Required response when "response_type" = "code id_token" and "state" is request parameter
        /// </summary>
        [FromForm(Name = "state")]
        public string State { get; set; } = null!;

        [FromForm(Name = "nonce")]
        public string? Nonce { get; set; }
    }

}
