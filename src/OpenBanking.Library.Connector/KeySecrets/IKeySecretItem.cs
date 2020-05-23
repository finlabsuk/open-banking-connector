// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Extensions;

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets
{
    /// <summary>
    ///     Item that can only be stored once in key secret vault. This interface is generic on implementing type TSelf which
    ///     should be a class
    ///     with only string properties.
    /// </summary>
    /// <remarks>
    ///     "Where" condition on interface enforces that TSelf is *a* class which implements
    ///     this interface - it does not unfortunately enforce that TSelf is the *current* implementing
    ///     class or that it only contains string properties.
    /// </remarks>
    /// <typeparam name="TSelf"></typeparam>
    public interface IKeySecretItem<TSelf>
        where TSelf : class, IKeySecretItem<TSelf>
    {
        static string GetKey(string propertyName) =>
            $"{typeof(TSelf).Name.PascalOrCamelToKebabCase()}:{propertyName.PascalOrCamelToKebabCase()}";
    }
}
