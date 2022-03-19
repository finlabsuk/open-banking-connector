// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AccountAndTransactionModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response
{
    public class TransactionsResponse : AccountAndTransactionModelsPublic.OBReadTransaction6
    {
        internal TransactionsResponse(
            AccountAndTransactionModelsPublic.OBReadDataTransaction6 data,
            AccountAndTransactionModelsPublic.Links links,
            AccountAndTransactionModelsPublic.Meta meta) : base(data, links, meta) { }
    }
}
