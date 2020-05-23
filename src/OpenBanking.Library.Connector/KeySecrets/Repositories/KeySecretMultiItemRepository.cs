// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories
{
    internal class KeySecretMultiItemRepository<TItem> : KeySecretMultiItemReadOnlyRepository<TItem>,
        IKeySecretMultiItemRepository<TItem>
        where TItem : class, IKeySecretItemWithId<TItem>
    {
        private readonly IKeySecretProvider _keySecretProvider;

        public KeySecretMultiItemRepository(IKeySecretProvider keySecretProvider) : base(
            (IKeySecretReadOnlyProvider) keySecretProvider)
        {
            _keySecretProvider = keySecretProvider ?? throw new ArgumentNullException(nameof(keySecretProvider));
        }

        public async Task UpsertAsync(TItem instance)
        {
            foreach (PropertyInfo property in typeof(TItem).GetProperties())
            {
                string key = IKeySecretItemWithId<TItem>.GetKey(
                    id: instance.Id,
                    propertyName: property.Name);
                KeySecret keySecret = new KeySecret(key: key, value: (string) property.GetValue(instance));
                await _keySecretProvider.SetKeySecretAsync(keySecret);
            }
        }
    }
}
