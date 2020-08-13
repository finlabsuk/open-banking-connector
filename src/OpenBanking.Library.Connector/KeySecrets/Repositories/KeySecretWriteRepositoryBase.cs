// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories
{
    /// <summary>
    ///     Code common to <see cref="KeySecretMultiItemWriteRepository" /> and <see cref="KeySecretWriteRepository" />
    ///     classes.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public abstract class KeySecretWriteRepositoryBase<TItem>
        where TItem : class
    {
        private readonly IKeySecretProvider _keySecretProvider;

        protected KeySecretWriteRepositoryBase(IKeySecretProvider keySecretProvider)
        {
            _keySecretProvider = keySecretProvider ?? throw new ArgumentNullException(nameof(keySecretProvider));
        }

        protected async Task UpsertAsync(Func<string, string> keyFcn, TItem instance, bool writeIdProperty)
        {
            foreach (PropertyInfo property in typeof(TItem).GetProperties())
            {
                if (property.PropertyType == typeof(string))
                {
                    if (!(property.Name == "Id" && writeIdProperty == false))
                    {
                        await SetAsync(keyFcn: keyFcn, property: property, instance: instance);
                    }
                }
                else if (property.PropertyType == typeof(List<string>))
                {
                    await SetListAsync(keyFcn: keyFcn, property: property, instance: instance);
                }
                else
                {
                    throw new InvalidDataException(
                        $"Properties of type {typeof(TItem)} are stored as key secrets and should be of either string or List<string> type.");
                }
            }
        }

        private async Task SetAsync(Func<string, string> keyFcn, PropertyInfo property, TItem instance)
        {
            string key = keyFcn(property.Name);
            if (property.PropertyType != typeof(string))
            {
                throw new InvalidOperationException("Invalid parameter type.");
            }

            KeySecret keySecret = new KeySecret(key: key, value: (string) property.GetValue(instance));
            await _keySecretProvider.SetKeySecretAsync(keySecret);
        }

        private async Task SetListAsync(Func<string, string> keyFcn, PropertyInfo property, TItem instance)
        {
            string key = keyFcn(property.Name);
            if (property.PropertyType != typeof(List<string>))
            {
                throw new InvalidOperationException("Invalid parameter type.");
            }

            List<string> value = (List<string>) property.GetValue(instance);
            KeySecret keySecret = new KeySecret(key: key, value: string.Join(separator: " ", values: value));
            await _keySecretProvider.SetKeySecretAsync(keySecret);
        }
    }
}
