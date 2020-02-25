// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace FinnovationLabs.OpenBanking.Library.Connector.Configuration
{
    public sealed class EnvironmentVariablesConfigurationProvider : IConfigurationProvider
    {
        private const string Root = "OpenBankingConnector";
        private readonly IDictionary _environmentVariables;

        public EnvironmentVariablesConfigurationProvider()
            : this(Environment.GetEnvironmentVariables())
        {
        }

        internal EnvironmentVariablesConfigurationProvider(IDictionary environmentVariables)
        {
            _environmentVariables = environmentVariables.ArgNotNull(nameof(environmentVariables));
        }

        public RuntimeConfiguration GetRuntimeConfiguration()
        {
            var evs = GetEnvironmentVariables(Root);

            var result = new DefaultConfigurationProvider().GetRuntimeConfiguration();

            result.DefaultCurrency = GetEnvVarValue(evs, $"{Root}:DefaultCurrency") ?? result.DefaultCurrency;
            result.SoftwareId = GetEnvVarValue(evs, $"{Root}:SoftwareId") ?? result.SoftwareId;

            return result;
        }

        private string GetEnvVarValue(Dictionary<string, string> variables, string name)
        {
            if (variables.TryGetValue(name, out var value))
            {
                return value;
            }

            return null;
        }

        private Dictionary<string, string> GetEnvironmentVariables(string root)
        {
            var result = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

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
