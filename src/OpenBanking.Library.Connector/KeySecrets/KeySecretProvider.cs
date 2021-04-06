// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets
{
    public enum KeySecretProvider
    {
        Azure
    }

    public static class KeySecretProviderHelper
    {
        static KeySecretProviderHelper()
        {
            AllKeySecretProviders = Enum.GetValues(typeof(KeySecretProvider))
                .Cast<KeySecretProvider>();
        }

        public static IEnumerable<KeySecretProvider> AllKeySecretProviders { get; }

        public static KeySecretProvider KeySecretProvider(string keySecretsProviderString) =>
            Enum.Parse<KeySecretProvider>(keySecretsProviderString);
    }
}
