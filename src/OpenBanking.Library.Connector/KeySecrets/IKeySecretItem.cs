// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets
{
    /// <summary>
    ///     Item that can only be stored once in key secret vault. Implementing type
    ///     should be a class
    ///     with only string or IEnumerable<string> properties.
    /// </summary>
    public interface IKeySecretItem { }
}
