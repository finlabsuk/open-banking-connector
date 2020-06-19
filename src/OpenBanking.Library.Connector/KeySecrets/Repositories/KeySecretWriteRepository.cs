// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories
{
    /// <summary>
    ///     Default implementation of <see cref="IKeySecretWriteRepository" /> service.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class KeySecretWriteRepository<TItem> : KeySecretWriteRepositoryBase<TItem>,
        IKeySecretWriteRepository<TItem>
        where TItem : class, IKeySecretItem
    {
        public KeySecretWriteRepository(IKeySecretProvider keySecretProvider) : base(keySecretProvider) { }

        public Task UpsertAsync(TItem instance) => UpsertAsync(
            keyFcn: s => Helpers.KeyWithoutId<TItem>(propertyName: s),
            instance: instance,
            writeIdProperty: true);
    }
}
