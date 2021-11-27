// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Configuration
{
    /// <summary>
    ///     Settings provider for <see cref="SoftwareStatementProfilesSettings" /> which gets
    ///     settings form key secret provider
    /// </summary>
    public class
        KeySecretsSoftwareStatementProfilesSettingsProvider : ISettingsProvider<SoftwareStatementProfilesSettings>
    {
        private readonly SoftwareStatementProfilesSettings _settings;

        public KeySecretsSoftwareStatementProfilesSettingsProvider(
            ISettingsProvider<OpenBankingConnectorSettings> obcSettingsProvider,
            IKeySecretProvider keySecretsProvider)
        {
            // Read and validate settings on object creation (app startup).
            OpenBankingConnectorSettings obcSettings = obcSettingsProvider.GetSettings();
            obcSettings.ArgNotNull(nameof(obcSettings));

            _settings = new SoftwareStatementProfilesSettings();

            List<string> activeProfileIds = obcSettings.SoftwareStatementProfileIdsAsList.ToList();
            foreach (string id in activeProfileIds)
            {
                SoftwareStatementProfileWithOverrideProperties item = Helpers
                    .GetAsync<SoftwareStatementProfileWithOverrideProperties>(
                        s => Helpers.KeyWithId<SoftwareStatementProfilesSettings>(id, s),
                        keySecretsProvider)
                    .GetAwaiter()
                    .GetResult();
                _settings.TryAdd(id, item);
            }

            _settings.Validate();
        }

        public SoftwareStatementProfilesSettings GetSettings()
        {
            return _settings;
        }
    }
}
