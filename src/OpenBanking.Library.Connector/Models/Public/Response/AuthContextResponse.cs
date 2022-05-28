// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    public interface IAuthContextPublicQuery : IBaseQuery { }

    /// <summary>
    ///     Response to GetLocal
    /// </summary>
    public class AuthContextResponse : LocalObjectBaseResponse,
        IAuthContextPublicQuery
    {
        internal AuthContextResponse(Guid id, DateTimeOffset created, string? createdBy, string? reference) : base(
            id,
            created,
            createdBy,
            reference) { }

        /// <summary>
        ///     Optional list of warning messages from Open Banking Connector.
        /// </summary>
        public IList<string>? Warnings { get; }
    }
}
