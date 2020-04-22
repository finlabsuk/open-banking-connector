// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public interface ISharedContext
    {
        DateTimeOffset Created { get; set; }
        ICertificateReader CertificateReader { get; }
        IApiClient ApiClient { get; }
        IConfigurationProvider ConfigurationProvider { get; }
        IInstrumentationClient Instrumentation { get; }
        IKeySecretProvider KeySecretProvider { get; }
        IDbMultiEntityMethods DbContextService { get; }
        IDbEntityRepository<BankClientProfile> ClientProfileRepository { get; }
        IDbEntityRepository<SoftwareStatementProfile> SoftwareStatementRepository { get; }
        IDbEntityRepository<DomesticConsent> DomesticConsentRepository { get; }
        IEntityMapper EntityMapper { get; }
        IDbEntityRepository<ApiProfile> ApiProfileRepository { get; }
    }
}