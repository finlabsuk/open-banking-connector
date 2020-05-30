// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using Microsoft.Extensions.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.NetGenericHost.KeySecrets
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

            var secrets = Enumerable.Range(start: 1, count: Secrets.MaxSoftwareStatements)
                .SelectMany(i => GetSecrets(config: config, softwareStatement: i, obcConfig: obcConfig))
                .Where(s => s != null);

            return new MemoryKeySecretProvider(secrets);
        }

        private IEnumerable<KeySecret> GetSecrets(
            IConfiguration config,
            int softwareStatement,
            RuntimeConfiguration obcConfig)
        {
            var secrets = new[]
            {
                GetSecret(
                    config: config,
                    softwareStatement: softwareStatement,
                    name: Secrets.SoftwareStatement),
                GetSecret(
                    config: config,
                    softwareStatement: softwareStatement,
                    name: Secrets.SigningKeyId),
                GetSecret(
                    config: config,
                    softwareStatement: softwareStatement,
                    name: Secrets.SigningCertificateKey),
                GetSecret(
                    config: config,
                    softwareStatement: softwareStatement,
                    name: Secrets.SigningCertificate),
                GetSecret(
                    config: config,
                    softwareStatement: softwareStatement,
                    name: Secrets.TransportCertificateKey),
                GetSecret(
                    config: config,
                    softwareStatement: softwareStatement,
                    name: Secrets.TransportCertificate)
            };

            return secrets;
        }

        private KeySecret GetSecret(IConfiguration config, int softwareStatement, string name)
        {
            var configKey = Secrets.GetName(
                softwareStatementId: softwareStatement.ToString(),
                name: name);

            var s = config.GetValue<string>(configKey);

            return s != null
                ? new KeySecret(key: configKey, value: s)
                : null;
        }
    }
}
