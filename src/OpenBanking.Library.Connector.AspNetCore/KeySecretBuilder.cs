// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Microsoft.Extensions.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.AspNetCore
{
    internal class KeySecretBuilder
    {
        public IKeySecretProvider GetKeySecretProvider(IConfiguration config, RuntimeConfiguration obcConfig)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (obcConfig == null)
            {
                throw new ArgumentNullException(nameof(obcConfig));
            }

            var secrets = Enumerable.Range(1, KeySecrets.MaxSoftwareStatements)
                .SelectMany(i => GetSecrets(config, i, obcConfig)).Where(s => s != null);

            return new MemoryKeySecretProvider(secrets);
        }

        private IEnumerable<KeySecret> GetSecrets(IConfiguration config, int softwareStatement,
            RuntimeConfiguration obcConfig)
        {
            var secrets = new[]
            {
                GetSecret(config, softwareStatement, KeySecrets.SoftwareStatement),
                GetSecret(config, softwareStatement, KeySecrets.SigningKeyId),
                GetSecret(config, softwareStatement, KeySecrets.SigningCertificateKey),
                GetSecret(config, softwareStatement, KeySecrets.SigningCertificate),
                GetSecret(config, softwareStatement, KeySecrets.TransportCertificateKey),
                GetSecret(config, softwareStatement, KeySecrets.TransportCertificate)
            };

            return secrets;
        }

        private KeySecret GetSecret(IConfiguration config, int softwareStatement, string name)
        {
            var configKey = KeySecrets.GetName(softwareStatement.ToString(), name);

            var s = config.GetValue<string>(configKey);

            return s != null
                ? new KeySecret(configKey, s)
                : null;
        }
    }
}
