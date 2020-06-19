// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Extensions;

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets
{
    /// <summary>
    ///     Helper methods for Key Secrets.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        ///     Key containing ID used for storage of key secrets where the same item (set of secrets) can be stored
        ///     more than once.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="propertyName"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <returns></returns>
        public static string KeyWithId<TItem>(string id, string propertyName)
            where TItem : class, IKeySecretItemWithId =>
            $"{typeof(TItem).Name.PascalOrCamelToKebabCase()}:{id}:{propertyName.PascalOrCamelToKebabCase()}";

        /// <summary>
        ///     Key without ID used for storage of key secrets where the same item (set of secrets) can only be stored once only.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <typeparam name="TItem"></typeparam>
        /// <returns></returns>
        public static string KeyWithoutId<TItem>(string propertyName)
            where TItem : class, IKeySecretItem =>
            $"{typeof(TItem).Name.PascalOrCamelToKebabCase()}:{propertyName.PascalOrCamelToKebabCase()}";
    }
}
