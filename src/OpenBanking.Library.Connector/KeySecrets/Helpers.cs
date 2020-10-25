// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets
{
    /// <summary>
    ///     Helper methods for Key Secrets.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        ///     Key containing ID used for storage of key secrets where the same item (set of secrets) can be stored
        ///     more than once.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="propertyName"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <returns></returns>
        public static string KeyWithId<TItem>(string id, string propertyName)
            where TItem : class, IKeySecretItem =>
            $"obc:{typeof(TItem).Name.PascalOrCamelToKebabCase()}:{id}:{propertyName.PascalOrCamelToKebabCase()}";

        public static async Task UpsertAsync<TItem>(
            Func<string, string> keyFcn,
            TItem instance,
            bool writeIdProperty,
            IKeySecretProvider keySecretProvider)
        {
            foreach (PropertyInfo property in typeof(TItem).GetProperties())
            {
                if (property.PropertyType == typeof(string))
                {
                    if (!(property.Name == "Id" && writeIdProperty == false))
                    {
                        await SetAsync(
                            keyFcn: keyFcn,
                            property: property,
                            instance: instance,
                            keySecretProvider: keySecretProvider);
                    }
                }
                else if (property.PropertyType == typeof(List<string>))
                {
                    await SetListAsync(
                        keyFcn: keyFcn,
                        property: property,
                        instance: instance,
                        keySecretProvider: keySecretProvider);
                }
                else
                {
                    throw new InvalidDataException(
                        $"Properties of type {typeof(TItem)} are stored as key secrets and should be of either string or List<string> type.");
                }
            }
        }

        private static async Task SetAsync<TItem>(
            Func<string, string> keyFcn,
            PropertyInfo property,
            TItem instance,
            IKeySecretProvider keySecretProvider)
        {
            string key = keyFcn(property.Name);
            if (property.PropertyType != typeof(string))
            {
                throw new InvalidOperationException("Invalid parameter type.");
            }

            KeySecret keySecret = new KeySecret(key: key, value: (string) property.GetValue(instance));
            await keySecretProvider.SetKeySecretAsync(keySecret);
        }

        private static async Task SetListAsync<TItem>(
            Func<string, string> keyFcn,
            PropertyInfo property,
            TItem instance,
            IKeySecretProvider keySecretProvider)
        {
            string key = keyFcn(property.Name);
            if (property.PropertyType != typeof(List<string>))
            {
                throw new InvalidOperationException("Invalid parameter type.");
            }

            List<string> value = (List<string>) property.GetValue(instance);
            KeySecret keySecret = new KeySecret(key: key, value: string.Join(separator: " ", values: value));
            await keySecretProvider.SetKeySecretAsync(keySecret);
        }

        public static async Task<string> GetAsync(
            Func<string, string> keyFcn,
            PropertyInfo property,
            IKeySecretReadOnlyProvider keySecretReadOnlyProvider)
        {
            string key = keyFcn(property.Name);
            if (property.PropertyType != typeof(string))
            {
                throw new InvalidOperationException("Invalid parameter type.");
            }

            KeySecret keySecret = await keySecretReadOnlyProvider.GetKeySecretAsync(key);
            return keySecret.Value;
        }

        public static async Task<TItem> GetAsync<TItem>(
            Func<string, string> keyFcn,
            string idPropertyValue,
            IKeySecretReadOnlyProvider keySecretReadOnlyProvider)
        {
            List<object> values = new List<object>();
            foreach (PropertyInfo property in typeof(TItem).GetProperties())
            {
                if (property.PropertyType == typeof(string))
                {
                    if (property.Name == "Id" && idPropertyValue != null)
                    {
                        values.Add(idPropertyValue);
                    }
                    else
                    {
                        values.Add(
                            await GetAsync(
                                keyFcn: keyFcn,
                                property: property,
                                keySecretReadOnlyProvider: keySecretReadOnlyProvider));
                    }
                }
                else if (property.PropertyType == typeof(List<string>))
                {
                    values.Add(
                        await GetListAsync(
                            keyFcn: keyFcn,
                            property: property,
                            keySecretReadOnlyProvider: keySecretReadOnlyProvider));
                }
                else
                {
                    throw new InvalidDataException(
                        $"Properties of type {typeof(TItem)} are stored as key secrets and should be of either string or List<string> type.");
                }
            }

            return (TItem) Activator.CreateInstance(type: typeof(TItem), args: values.ToArray<object>());
        }

        public static async Task<IEnumerable<string>> GetListAsync(
            Func<string, string> keyFcn,
            PropertyInfo property,
            IKeySecretReadOnlyProvider keySecretReadOnlyProvider)
        {
            string key = keyFcn(property.Name);
            if (property.PropertyType != typeof(List<string>))
            {
                throw new InvalidOperationException("Invalid parameter type.");
            }

            KeySecret keySecret = await keySecretReadOnlyProvider.GetKeySecretAsync(key);
            List<string> valueAsList = keySecret.Value.Split(" ").ToList();
            return valueAsList;
        }
    }
}
