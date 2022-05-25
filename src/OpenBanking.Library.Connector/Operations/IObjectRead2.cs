// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal interface IObjectRead2<TPublicResponse>
    {
        Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ReadAsync(
            Guid consentId,
            string? externalApiAccountId,
            string? externalApiStatementId,
            string? fromBookingDateTime,
            string? toBookingDateTime,
            string? page,
            string? modifiedBy,
            string? publicRequestUrlWithoutQuery,
            string? queryString);
    }
}
