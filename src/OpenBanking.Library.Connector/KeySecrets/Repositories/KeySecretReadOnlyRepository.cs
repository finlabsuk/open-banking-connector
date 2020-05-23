// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories
{
    internal class KeySecretReadOnlyRepository<TItem> : IKeySecretReadOnlyRepository<TItem>
        where TItem : class, IKeySecretItem<TItem>
    {
        private readonly IKeySecretReadOnlyProvider _keySecretReadOnlyProvider;

        public KeySecretReadOnlyRepository(IKeySecretReadOnlyProvider keySecretReadOnlyProvider)
        {
            _keySecretReadOnlyProvider = keySecretReadOnlyProvider ??
                                         throw new ArgumentNullException(nameof(keySecretReadOnlyProvider));
        }

        public async Task<TItem> GetAsync()
        {
            List<string> values = new List<string>();
            foreach (PropertyInfo property in typeof(TItem).GetProperties())
            {
                string key = IKeySecretItem<TItem>.GetKey(propertyName: property.Name);
                KeySecret keySecret = await _keySecretReadOnlyProvider.GetKeySecretAsync(key);
                values.Add(keySecret.Value);
            }

            return (TItem) Activator.CreateInstance(type: typeof(TItem), args: values.ToArray<object>());
        }

        public async Task<string> GetAsync(string propertyName)
        {
            string key =
                IKeySecretItem<TItem>.GetKey(propertyName: propertyName);
            KeySecret keySecret = await _keySecretReadOnlyProvider.GetKeySecretAsync(key);
            return keySecret.Value;
        }
    }
}
