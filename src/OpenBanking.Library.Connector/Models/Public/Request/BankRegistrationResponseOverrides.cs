﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    /// <summary>
    ///     Class used to specify overrides to returned registration response
    ///     from bank API.
    ///     Sometimes values are missing and they can be added using this class to allow
    ///     OBC to have necessary information about a bank registration.
    ///     Default (null) property values do not lead to changes.
    /// </summary>
    public class BankRegistrationResponseOverrides
    {
        [JsonProperty("grant_types")]
        public IList<string>? GrantTypes { get; set; }
    }
}
