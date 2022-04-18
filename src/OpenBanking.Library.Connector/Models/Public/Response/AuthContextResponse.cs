// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    public interface IAuthContextPublicQuery : IBaseQuery { }

    /// <summary>
    ///     Response to GetLocal
    /// </summary>
    public class AuthContextResponse : BaseResponse,
        IAuthContextPublicQuery
    {
        internal AuthContextResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy) : base(id, created, createdBy) { }
    }
}
