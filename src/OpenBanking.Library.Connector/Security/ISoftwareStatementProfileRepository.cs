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
    public interface ISoftwareStatementProfileRepository
    {
        Task<SoftwareStatementProfile> GetAsync(string id);

        Task<IQueryable<SoftwareStatementProfile>> GetAsync(Expression<Func<SoftwareStatementProfile, bool>> predicate);

        Task<SoftwareStatementProfile> SetAsync(SoftwareStatementProfile profile);

        Task SaveChangesAsync();

        Task<bool> DeleteAsync(string id);

        Task<IList<string>> GetIdsAsync();
    }
}
