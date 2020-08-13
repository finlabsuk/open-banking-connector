// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace FinnovationLabs.OpenBanking.Library.Connector.Configuration
{
    public sealed class EnvironmentVariablesConfigurationProvider : IObcConfigurationProvider
    {
        private const string Root = "OpenBankingConnector";
        private readonly IDictionary _environmentVariables;

        public EnvironmentVariablesConfigurationProvider()
            : this(Environment.GetEnvironmentVariables()) { }

        internal EnvironmentVariablesConfigurationProvider(IDictionary environmentVariables)
        {
            _environmentVariables = environmentVariables.ArgNotNull(nameof(environmentVariables));
        }

        public ObcConfiguration GetObcConfiguration()
        {
            Dictionary<string, string> evs = GetEnvironmentVariables(Root);

            ObcConfiguration result = new DefaultConfigurationProvider().GetObcConfiguration();

            result.DefaultCurrency = GetEnvVarValue(variables: evs, name: $"{Root}:DefaultCurrency") ??
                                     result.DefaultCurrency;
            result.SoftwareId = GetEnvVarValue(variables: evs, name: $"{Root}:SoftwareId") ?? result.SoftwareId;
            result.EnsureDbCreated = GetEnvVarValue(variables: evs, name: $"{Root}:EnsureDbCreated") ??
                                     result.EnsureDbCreated;


            return result;
        }

        private string GetEnvVarValue(Dictionary<string, string> variables, string name)
        {
            if (variables.TryGetValue(key: name, value: out string value))
            {
                return value;
            }

            return null;
        }

        private Dictionary<string, string> GetEnvironmentVariables(string root)
        {
            Dictionary<string, string> result =
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
