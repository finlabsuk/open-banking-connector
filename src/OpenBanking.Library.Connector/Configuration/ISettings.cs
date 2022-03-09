// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Configuration
{
    /// <summary>
    ///     Settings section (group) used by Open Banking Connector. Represents
    ///     a group of related settings at the same level of nesting e.g.
    ///     section in appSettings.json or user secrets. For flexibility, these classes do not depend on .NET Generic Host
    ///     options pattern features.
    ///     In the case of settings provided by key secrets, all properties in <see cref="TSelf" /> should use a type
    ///     representable by string values (e.g. bool, string, etc).
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    public interface ISettings<out TSelf>
        where TSelf : ISettings<TSelf>
    {
        string SettingsGroupName { get; }

        /// <summary>
        ///     Validate settings. Returns itself to allow for method chaining.
        /// </summary>
        /// <returns></returns>
        TSelf Validate();
    }
}
