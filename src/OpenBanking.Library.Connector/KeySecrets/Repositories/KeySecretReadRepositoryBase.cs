// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories
{
    /// <summary>
    ///     Code common to <see cref="KeySecretMultiItemReadRepository" /> and <see cref="KeySecretReadRepository" /> classes.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public abstract class KeySecretReadRepositoryBase<TItem> where TItem : class
    {
        private readonly IKeySecretReadOnlyProvider _keySecretReadOnlyProvider;

        protected KeySecretReadRepositoryBase(IKeySecretReadOnlyProvider keySecretReadOnlyProvider)
        {
            _keySecretReadOnlyProvider = keySecretReadOnlyProvider ??
                                         throw new ArgumentNullException(nameof(keySecretReadOnlyProvider));
        }

        protected async Task<string> GetAsync(Func<string, string> keyFcn, PropertyInfo property)
        {
            string key = keyFcn(property.Name);
            if (property.PropertyType != typeof(string))
            {
                throw new InvalidOperationException("Invalid parameter type.");
            }

            KeySecret keySecret = await _keySecretReadOnlyProvider.GetKeySecretAsync(key);
            return keySecret.Value;
        }

        protected async Task<TItem> GetAsync(Func<string, string> keyFcn, string idPropertyValue)
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
                                property: property));
                    }
                }
                else if (property.PropertyType == typeof(List<string>))
                {
                    values.Add(
                        await GetListAsync(
                            keyFcn: keyFcn,
                            property: property));
                }
                else
                {
                    throw new InvalidDataException(
                        $"Properties of type {typeof(TItem)} are stored as key secrets and should be of either string or List<string> type.");
                }
            }

            return (TItem) Activator.CreateInstance(type: typeof(TItem), args: values.ToArray<object>());
        }

        protected async Task<IEnumerable<string>> GetListAsync(
            Func<string, string> keyFcn,
            PropertyInfo property)
        {
            string key = keyFcn(property.Name);
            if (property.PropertyType != typeof(List<string>))
            {
                throw new InvalidOperationException("Invalid parameter type.");
            }

            KeySecret keySecret = await _keySecretReadOnlyProvider.GetKeySecretAsync(key);
            List<string> valueAsList = keySecret.Value.Split(" ").ToList();
            return valueAsList;
        }
    }
}
