// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Configuration
{
    /// <summary>
    ///     An <see cref="ISettingsProvider{TSettings}" /> class:
    ///     * provides settings which have been validated at app startup and which do not change during app lifetime (the app
    ///     should arted to pick up settings changes)
    ///     * is not required to depend on .NET Generic Host options pattern features
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public interface ISettingsProvider<out TSettings>
        where TSettings : ISettings<TSettings>
    {
        TSettings GetSettings();
    }
}
