// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Security.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public interface ISharedContext
    {
        BaseDbContext DbContext { get; }
        DateTimeOffset Created { get; set; }
        ICertificateReader CertificateReader { get; }
        IApiClient ApiClient { get; }
        IConfigurationProvider ConfigurationProvider { get; }
        IInstrumentationClient Instrumentation { get; }
        IKeySecretProvider KeySecretProvider { get; }
        IOpenBankingClientProfileRepository ClientProfileRepository { get; }
        IDbEntityRepository<SoftwareStatementProfile> SoftwareStatementRepository { get; }
        IDomesticConsentRepository DomesticConsentRepository { get; }
        IEntityMapper EntityMapper { get; }
        IApiProfileRepository ApiProfileRepository { get; }
    }
}