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
    ///     All properties in <see cref="TSelf" /> should ultimately resolve to strings for the case of key secrets settings.
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    public interface ISettings<out TSelf>
        where TSelf : ISettings<TSelf>
    {
        string SettingsSectionName { get; }

        TSelf Validate();
    }
}
