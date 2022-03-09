// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Configuration
{
    /// <summary>
    ///     Legacy settings provider that gets settings from environment variables
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public sealed class EnvironmentVariablesSettingsProvider<TSettings> : ISettingsProvider<TSettings>
        where TSettings : class, ISettings<TSettings>, new()
    {
        private readonly IDictionary _environmentVariables;
        private readonly string _root = new TSettings().SettingsGroupName;

        public EnvironmentVariablesSettingsProvider()
            : this(Environment.GetEnvironmentVariables()) { }

        internal EnvironmentVariablesSettingsProvider(IDictionary environmentVariables)
        {
            _environmentVariables = environmentVariables.ArgNotNull(nameof(environmentVariables));
        }

        public TSettings GetSettings()
        {
            Dictionary<string, string> evs = GetEnvironmentVariables(_root);

            var result = new TSettings();

            // TODO: If we keep this class, code below is placeholder and needs generalisation to all TSettings types
            if (result is OpenBankingConnectorSettings resultObcSettings)
            {
                resultObcSettings.SoftwareStatementProfileIds =
                    GetEnvVarValue(evs, $"{_root}:SoftwareStatementProfileIds") ??
                    resultObcSettings.SoftwareStatementProfileIds;
                return (resultObcSettings as TSettings)!;
            }

            return result;
        }

        private string? GetEnvVarValue(Dictionary<string, string> variables, string name)
        {
            if (variables.TryGetValue(name, out string value))
            {
                return value;
            }

            return null;
        }

        private Dictionary<string, string> GetEnvironmentVariables(string root)
        {
            var result =
                new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            foreach (DictionaryEntry ev in _environmentVariables)
            {
                if (ev.Key.ToString().StartsWith(root))
                {
                    result[ev.Key.ToString()] = ev.Value.ToString();
                }
            }

            return result;
        }
    }
}
