// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal interface IObjectGet<TPublicQuery, TPublicResponse>
    {
        Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> GetAsync(
            Guid id,
            string? modifiedBy,
            string? apiResponseWriteFile,
            string? apiResponseOverrideFile);

        Task<(IQueryable<TPublicResponse> response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages
                )>
            GetAsync(Expression<Func<TPublicQuery, bool>> predicate);
    }
}
