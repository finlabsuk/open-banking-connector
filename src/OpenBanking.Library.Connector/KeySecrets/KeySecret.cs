// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets
{
    public class KeySecret
    {
        public const string DefaultVaultName = "OpenBankingConnector";

        public KeySecret(string key, string value)
            : this(vaultName: DefaultVaultName, key: key, value: value) { }

        public KeySecret(string vaultName, string key, string value)
        {
            VaultName = vaultName.ArgNotNull(nameof(vaultName));
            Key = key.ArgNotNull(nameof(key));
            Value = value.ArgNotNull(nameof(value));
        }

        public string Key { get; }

        public string VaultName { get; }

        public string Value { get; }
    }
}
