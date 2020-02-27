// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security.PaymentInitiation
{
    public interface IApiProfileRepository
    {
        Task<ApiProfile> GetAsync(string id);

        Task<IQueryable<ApiProfile>> GetAsync(Expression<Func<ApiProfile, bool>> predicate);

        Task<ApiProfile> SetAsync(ApiProfile profile);

        Task<bool> DeleteAsync(string id);

        Task<IList<string>> GetIdsAsync();
    }
}
