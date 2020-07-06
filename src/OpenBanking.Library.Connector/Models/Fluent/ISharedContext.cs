// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using SoftwareStatementProfile =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.SoftwareStatementProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public interface ISharedContext
    {
        DateTimeOffset Created { get; set; }
        ICertificateReader CertificateReader { get; }
        IApiClient ApiClient { get; }
        IObcConfigurationProvider ConfigurationProvider { get; }
        IInstrumentationClient Instrumentation { get; }
        IKeySecretReadOnlyProvider KeySecretReadOnlyProvider { get; }
        IDbMultiEntityMethods DbContextService { get; }
        IDbEntityRepository<BankClientProfile> ClientProfileRepository { get; }
        ISoftwareStatementProfileService SoftwareStatementProfileService { get; }
        IKeySecretWriteRepository<ActiveSoftwareStatementProfiles> ActiveSRRepo { get; }
        IKeySecretMultiItemReadRepository<SoftwareStatementProfile> SReadOnlyRepo { get; }
        IKeySecretMultiItemWriteRepository<SoftwareStatementProfile> SRepo { get; }
        IKeySecretReadRepository<ActiveSoftwareStatementProfiles> ActiveSReadOnlyRepo { get; }
        IDbEntityRepository<DomesticConsent> DomesticConsentRepository { get; }
        IEntityMapper EntityMapper { get; }
        IDbEntityRepository<ApiProfile> ApiProfileRepository { get; }
    }
}
