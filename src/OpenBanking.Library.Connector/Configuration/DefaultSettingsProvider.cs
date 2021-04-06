// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Configuration
{
    /// <summary>
    ///     Default settings provider that simply validates settings on creation
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public class DefaultSettingsProvider<TSettings> : ISettingsProvider<TSettings>
        where TSettings : ISettings<TSettings>, new()
    {
        private readonly TSettings _settings;

        public DefaultSettingsProvider(TSettings settings)
        {
            _settings = settings
                .Validate();
        }

        public TSettings GetSettings()
        {
            return _settings;
        }
    }
}
