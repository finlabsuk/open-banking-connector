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
    public class SharedContext: ISharedContext
    {
        internal SharedContext(BaseDbContext dbContext, ICertificateReader certificateReader, IApiClient apiClient, IConfigurationProvider configurationProvider, IInstrumentationClient instrumentation, IKeySecretProvider keySecretProvider, IOpenBankingClientProfileRepository clientProfileRepository, IDbEntityRepository<SoftwareStatementProfile> softwareStatementRepository, IDomesticConsentRepository domesticConsentRepository, IEntityMapper entityMapper, IApiProfileRepository apiProfileRepository)
        {
            DbContext = dbContext;
            CertificateReader = certificateReader;
            ApiClient = apiClient;
            ConfigurationProvider = configurationProvider;
            Instrumentation = instrumentation;
            KeySecretProvider = keySecretProvider;
            ClientProfileRepository = clientProfileRepository;
            SoftwareStatementRepository = softwareStatementRepository;
            DomesticConsentRepository = domesticConsentRepository;
            EntityMapper = entityMapper;
            ApiProfileRepository = apiProfileRepository;
        }

        public BaseDbContext DbContext { get; }
        public DateTimeOffset Created { get; set; }
        public ICertificateReader CertificateReader { get; }
        public IApiClient ApiClient { get; }
        public IConfigurationProvider ConfigurationProvider { get; }
        public IInstrumentationClient Instrumentation { get; }
        public IKeySecretProvider KeySecretProvider { get; }
        public IOpenBankingClientProfileRepository ClientProfileRepository { get; }
        public IDbEntityRepository<SoftwareStatementProfile> SoftwareStatementRepository { get; }
        public IDomesticConsentRepository DomesticConsentRepository { get; }
        public IEntityMapper EntityMapper { get; }
        public IApiProfileRepository ApiProfileRepository { get; }
    }
}
