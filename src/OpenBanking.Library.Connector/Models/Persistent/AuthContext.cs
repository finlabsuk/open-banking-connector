﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class AuthContext :
        EntityBase,
        ISupportsFluentDeleteLocal<AuthContext>
    {
        /// <summary>
        ///     Token endpoint response. If null, indicates auth not successfully completed.
        /// </summary>
        public ReadWriteProperty<TokenEndpointResponse?> TokenEndpointResponse { get; set; } = null!;
    }
}