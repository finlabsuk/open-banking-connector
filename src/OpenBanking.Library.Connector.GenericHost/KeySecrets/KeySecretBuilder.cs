// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using Microsoft.Extensions.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.KeySecrets
{
    internal class KeySecretBuilder
    {
        public IKeySecretProvider GetKeySecretProvider(IConfiguration config, ObcConfiguration obcConfig)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (obcConfig == null)
            {
                throw new ArgumentNullException(nameof(obcConfig));
            }

            List<KeySecret> secrets = GetItemSecrets<ActiveSoftwareStatementProfiles>(config)
                .Where(s => s != null)
                .ToList();

            KeySecret? profileKey = secrets.Find(
                x => x.Key == Connector.KeySecrets.Helpers.KeyWithoutId<ActiveSoftwareStatementProfiles>(
                    nameof(ActiveSoftwareStatementProfiles.ProfileIds)));
            string? profileId = profileKey.Value;

            secrets.AddRange(GetItemWIthIdSecrets<SoftwareStatementProfile>(configuration: config, id: profileId));

            return new MemoryKeySecretProvider(secrets);
        }

        private IEnumerable<KeySecret> GetItemSecrets<TItem>(IConfiguration configuration)
            where TItem : class, IKeySecretItem
        {
            List<KeySecret> secrets = new List<KeySecret>();
            foreach (PropertyInfo property in typeof(TItem).GetProperties())
            {
                if (!(
                    property.PropertyType == typeof(string) ||
                    property.PropertyType == typeof(List<string>)
                ))
                {
                    throw new InvalidDataException(
                        $"Properties of type {typeof(TItem)} are stored as key secrets and should be of either string or List<string> type.");
                }

                string? key = Connector.KeySecrets.Helpers.KeyWithoutId<TItem>(property.Name);
                string? s = configuration.GetValue<string>(key);
                KeySecret? newSecret = s != null
                    ? new KeySecret(key: key, value: s)
                    : null;
                secrets.Add(newSecret);
            }

            return secrets;
        }

        private IEnumerable<KeySecret> GetItemWIthIdSecrets<TItem>(IConfiguration configuration, string id)
            where TItem : class, IKeySecretItemWithId
        {
            List<KeySecret> secrets = new List<KeySecret>();
            foreach (PropertyInfo property in typeof(TItem).GetProperties())
            {
                if (!(
                    property.PropertyType == typeof(string) ||
                    property.PropertyType == typeof(List<string>)
                ))
                {
                    throw new InvalidDataException(
                        $"Properties of type {typeof(TItem)} are stored as key secrets and should be of either string or List<string> type.");
                }

                string? key = Connector.KeySecrets.Helpers.KeyWithId<TItem>(id: id, propertyName: property.Name);

                string? s = property.Name switch
                {
                    "Id" => id,
                    _ => configuration.GetValue<string>(key)
                };
                KeySecret? newSecret = s != null
                    ? new KeySecret(key: key, value: s)
                    : null;
                secrets.Add(newSecret);
            }

            return secrets;
        }
    }
}
