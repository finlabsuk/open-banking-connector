// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Configuration
{
    /// <summary>
    ///     Settings provider which validates settings on creation so
    ///     app does not run with faulty settings.
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public interface ISettingsProvider<out TSettings>
        where TSettings : ISettings<TSettings>
    {
        TSettings GetSettings();
    }
}
