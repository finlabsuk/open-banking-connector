// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets
{
    /// <summary>
    ///     Item that can only stored more than once in key secret vault as ID of item is included in key string. Implementing
    ///     type
    ///     should be a class
    ///     with only string or IEnumerable<string> properties.
    /// </summary>
    public interface IKeySecretItemWithId
    {
        string Id { get; }
    }
}
