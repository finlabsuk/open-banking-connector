// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal class
    ConsentCommon<TEntity, TPublicRequest, TPublicResponse, TApiRequest, TApiResponse>
    where TEntity : class, IEntity
    where TPublicRequest : ConsentBase
    where TApiRequest : class
    where TApiResponse : class, ISupportsValidation
{
    private readonly IDbReadOnlyEntityMethods<BankRegistrationEntity> _bankRegistrationMethods;
    private readonly IInstrumentationClient _instrumentationClient;

    public ConsentCommon(
        IDbReadOnlyEntityMethods<BankRegistrationEntity> bankRegistrationMethods,
        IInstrumentationClient instrumentationClient)
    {
        _bankRegistrationMethods = bankRegistrationMethods;
        _instrumentationClient = instrumentationClient;
    }

    public async
        Task<(BankRegistrationEntity bankRegistration, string tokenEndpoint,
            SoftwareStatementEntity softwareStatement)> GetBankRegistration(Guid bankRegistrationId)
    {
        // Load BankRegistration
        BankRegistrationEntity bankRegistration =
            await _bankRegistrationMethods
                .DbSetNoTracking
                .Include(o => o.SoftwareStatementNavigation)
                .SingleOrDefaultAsync(x => x.Id == bankRegistrationId) ??
            throw new KeyNotFoundException(
                $"No record found for BankRegistrationId {bankRegistrationId} specified by request.");
        string tokenEndpoint = bankRegistration.TokenEndpoint;
        SoftwareStatementEntity softwareStatementEntity = bankRegistration.SoftwareStatementNavigation!;

        return (bankRegistration, tokenEndpoint, softwareStatementEntity);
    }
}
