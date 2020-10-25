// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets;
using Microsoft.Extensions.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.KeySecrets
{
    internal class KeySecretBuilder
    {
        public IList<KeySecret> GetKeySecretProvider(IConfiguration config, IEnumerable<string> profileIds)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            List<KeySecret> secrets = new List<KeySecret>();
            foreach (string id in profileIds)
            {
                IEnumerable<KeySecret> secretsForId = GetItemWIthIdSecrets<SoftwareStatementProfile>(
                    configuration: config,
                    id: id);
                secrets.AddRange(secretsForId);
            }

            return secrets;
        }

        private IEnumerable<KeySecret> GetItemWIthIdSecrets<TItem>(IConfiguration configuration, string id)
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

                string key = Connector.KeySecrets.Helpers.KeyWithId<TItem>(id: id, propertyName: property.Name);

                string s = property.Name switch
                {
                    "Id" => id,
                    _ => configuration.GetValue<string>(key) ??
                         throw new KeyNotFoundException($"Cannot find key secret with key {key}.")
                };
                KeySecret newSecret = new KeySecret(key: key, value: s);
                secrets.Add(newSecret);
            }

            return secrets;
        }
    }
}
