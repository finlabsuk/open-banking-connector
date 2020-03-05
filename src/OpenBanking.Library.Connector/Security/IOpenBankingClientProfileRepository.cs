// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security
{
    public interface IOpenBankingClientProfileRepository
    {
        Task<BankClientProfile> GetAsync(string id);

        Task<IQueryable<BankClientProfile>> GetAsync(Expression<Func<BankClientProfile, bool>> predicate);

        Task<BankClientProfile> SetAsync(BankClientProfile profile);

        Task<bool> DeleteAsync(string id);

        Task<IList<string>> GetIdsAsync();
    }
}
