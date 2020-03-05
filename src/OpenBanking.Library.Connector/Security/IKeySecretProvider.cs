// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security
{
    public interface IKeySecretProvider
    {
        Task<KeySecret> GetKeySecretAsync(string vaultName, string key);

        Task<KeySecret> GetKeySecretAsync(string key);
    }
}
