﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Access
{
    /// <summary>
    ///     Interface of repo service for key secret items. A key secret item is a class whose properties are
    ///     stored as key secrets.
    ///     This repo is a read repo for items which are placed in the key secret store multiple times;
    ///     item ID is included in each key to distinguish items.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface IReadOnlyKeySecretItemRepository<TItem>
        where TItem : class, IKeySecretItem
    {
        Task<TItem> GetAsync(string id);
    }
}
