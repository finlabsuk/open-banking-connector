// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories
{
    /// <summary>
    ///     Default implementation of <see cref="IKeySecretMultiItemWriteRepository" /> service.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class KeySecretMultiItemWriteRepository<TItem> : KeySecretWriteRepositoryBase<TItem>,
        IKeySecretMultiItemWriteRepository<TItem>
        where TItem : class, IKeySecretItemWithId
    {
        public KeySecretMultiItemWriteRepository(IKeySecretProvider keySecretProvider) : base(keySecretProvider) { }

        public async Task UpsertAsync(TItem instance) => await UpsertAsync(
            keyFcn: s => Helpers.KeyWithId<TItem>(id: instance.Id, propertyName: s),
            instance: instance,
            writeIdProperty: false);
    }
}
