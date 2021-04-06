// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Repositories
{
    /// <summary>
    ///     Item stored in repository. Requirement is that it must have a string ID.
    /// </summary>
    public interface IRepositoryItem
    {
        /// <summary>
        ///     Id of Repository Item.
        /// </summary>
        string Id { get; }
    }
}
