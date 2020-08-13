// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories
{
    /// <summary>
    ///     Default implementation of <see cref="IKeySecretMultiItemReadRepository" />  service.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class KeySecretMultiItemReadRepository<TItem> : KeySecretReadRepositoryBase<TItem>,
        IKeySecretMultiItemReadRepository<TItem>
        where TItem : class, IKeySecretItemWithId
    {
        public KeySecretMultiItemReadRepository(IKeySecretReadOnlyProvider keySecretReadOnlyProvider) : base(
            keySecretReadOnlyProvider) { }

        public Task<string> GetAsync(string id, string propertyName) =>
            GetAsync(
                keyFcn: s => Helpers.KeyWithId<TItem>(id: id, propertyName: s),
                property: typeof(TItem).GetProperty(propertyName));

        public Task<IEnumerable<string>> GetListAsync(string id, string propertyName) =>
            GetListAsync(
                keyFcn: s => Helpers.KeyWithId<TItem>(id: id, propertyName: s),
                property: typeof(TItem).GetProperty(propertyName));

        public Task<TItem> GetAsync(string id) =>
            GetAsync(keyFcn: s => Helpers.KeyWithId<TItem>(id: id, propertyName: s), idPropertyValue: id);
    }
}
