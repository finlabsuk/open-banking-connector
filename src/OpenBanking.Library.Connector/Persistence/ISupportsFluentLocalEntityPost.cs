﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    /// <summary>
    ///     "Post-only" type with public interface. Post-only types are similar to entity types with public interface (
    ///     <see cref="ISupportsFluentDeleteLocal{TSelf,TPublicRequest,TPublicResponse,TPublicQuery}" />) but are
    ///     not directly persisted to DB and can only be POSTed.
    /// </summary>
    internal interface ISupportsFluentLocalEntityPost<in TPublicRequest, out TPublicResponse>
    {
        TPublicResponse PublicPostResponse { get; }

        /// <summary>
        ///     Initialise entity based on request object
        /// </summary>
        /// <param name="request"></param>
        /// <param name="createdBy"></param>
        /// <param name="timeProvider"></param>
        void Initialise(
            TPublicRequest request,
            string? createdBy,
            ITimeProvider timeProvider);
    }
}