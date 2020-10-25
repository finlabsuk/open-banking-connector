// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Access
{
    /// <summary>
    ///     Default implementation of <see cref="IReadOnlyKeySecretItemRepository{TItem}" />  service.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class ReadOnlyKeySecretItemRepository<TItem> : IReadOnlyKeySecretItemRepository<TItem>
        where TItem : class, IKeySecretItem
    {
        private readonly IKeySecretReadOnlyProvider _keySecretReadOnlyProvider;

        public ReadOnlyKeySecretItemRepository(IKeySecretReadOnlyProvider keySecretReadOnlyProvider)
        {
            _keySecretReadOnlyProvider = keySecretReadOnlyProvider;
        }

        public Task<string> GetAsync(string id, string propertyName) =>
            Helpers.GetAsync(
                keyFcn: s => Helpers.KeyWithId<TItem>(id: id, propertyName: s),
                property: typeof(TItem).GetProperty(propertyName),
                keySecretReadOnlyProvider: _keySecretReadOnlyProvider);

        public Task<IEnumerable<string>> GetListAsync(string id, string propertyName) =>
            Helpers.GetListAsync(
                keyFcn: s => Helpers.KeyWithId<TItem>(id: id, propertyName: s),
                property: typeof(TItem).GetProperty(propertyName),
                keySecretReadOnlyProvider: _keySecretReadOnlyProvider);

        public Task<TItem> GetAsync(string id) =>
            Helpers.GetAsync<TItem>(
                keyFcn: s => Helpers.KeyWithId<TItem>(id: id, propertyName: s),
                idPropertyValue: id,
                keySecretReadOnlyProvider: _keySecretReadOnlyProvider);
    }
}
