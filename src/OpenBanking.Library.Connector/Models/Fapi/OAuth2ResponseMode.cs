﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;

[JsonConverter(typeof(StringEnumConverter))]
public enum OAuth2ResponseMode
{
    [EnumMember(Value = "query")]
    Query,

    [EnumMember(Value = "fragment")]
    Fragment,

    [EnumMember(Value = "form_post")]
    FormPost
}
