﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Azure;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.KeySecrets
{
    public static class KeySecretProviders
    {
        public static IDictionary<KeySecretProvider, IGenericHostKeySecretProvider> Providers { get; } =
            new Dictionary<KeySecretProvider, IGenericHostKeySecretProvider>
            {
                { KeySecretProvider.Azure, new AzureGenericHostKeySecretProvider() }
            };
    }
}
