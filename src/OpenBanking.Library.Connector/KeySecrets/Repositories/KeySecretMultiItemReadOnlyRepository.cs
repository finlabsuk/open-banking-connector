// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories
{
    internal class KeySecretMultiItemReadOnlyRepository<TItem> : IKeySecretMultiItemReadOnlyRepository<TItem>
        where TItem : class, IKeySecretItemWithId<TItem>
    {
        private readonly IKeySecretReadOnlyProvider _keySecretReadOnlyProvider;

        public KeySecretMultiItemReadOnlyRepository(IKeySecretReadOnlyProvider keySecretReadOnlyProvider)
        {
            _keySecretReadOnlyProvider = keySecretReadOnlyProvider ??
                                         throw new ArgumentNullException(nameof(keySecretReadOnlyProvider));
        }

        public async Task<string> GetAsync(string id, string propertyName)
        {
            string key =
                IKeySecretItemWithId<TItem>.GetKey(id: id, propertyName: propertyName);
            KeySecret keySecret = await _keySecretReadOnlyProvider.GetKeySecretAsync(key);
            return keySecret.Value;
        }

        public async Task<TItem> GetAsync(string id)
        {
            List<string> values = new List<string>();
            foreach (PropertyInfo property in typeof(TItem).GetProperties())
            {
                string key = IKeySecretItemWithId<TItem>.GetKey(
                    id: id,
                    propertyName: property.Name);
                KeySecret keySecret = await _keySecretReadOnlyProvider.GetKeySecretAsync(key);
                values.Add(keySecret.Value);
            }

            return (TItem) Activator.CreateInstance(type: typeof(TItem), values);
        }
    }
}
