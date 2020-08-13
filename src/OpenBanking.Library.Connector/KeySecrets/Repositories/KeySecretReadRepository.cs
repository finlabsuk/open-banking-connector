// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories
{
    /// <summary>
    ///     Default implementation of <see cref="IKeySecretReadRepository" /> service.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class KeySecretReadRepository<TItem> : KeySecretReadRepositoryBase<TItem>,
        IKeySecretReadRepository<TItem>
        where TItem : class, IKeySecretItem
    {
        public KeySecretReadRepository(IKeySecretReadOnlyProvider keySecretReadOnlyProvider) : base(
            keySecretReadOnlyProvider) { }

        public Task<string> GetAsync(string propertyName) =>
            GetAsync(
                keyFcn: s => Helpers.KeyWithoutId<TItem>(propertyName: s),
                property: typeof(TItem).GetProperty(propertyName));

        public Task<IEnumerable<string>> GetListAsync(string propertyName) =>
            GetListAsync(
                keyFcn: s => Helpers.KeyWithoutId<TItem>(propertyName: s),
                property: typeof(TItem).GetProperty(propertyName));

        public Task<TItem> GetAsync() => base.GetAsync(
            keyFcn: s => Helpers.KeyWithoutId<TItem>(propertyName: s),
            idPropertyValue: null);
    }
}
