// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security
{
    public interface IDomesticConsentRepository
    {
        Task<DomesticConsent> GetAsync(string id);

        Task<IQueryable<DomesticConsent>> GetAsync(Expression<Func<DomesticConsent, bool>> predicate);

        Task<DomesticConsent> SetAsync(DomesticConsent value);

        Task<bool> DeleteAsync(string id);

        Task<IList<string>> GetIdsAsync();
    }
}
