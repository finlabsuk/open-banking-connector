// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal interface IExternalRead<TPublicResponse>
    {
        Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ReadAsync(
            string externalId,
            Guid consentId,
            string? modifiedBy);
    }
}
