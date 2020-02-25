// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Security;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public class OpenBankingContext
    {
        internal OpenBankingContext(IConfigurationProvider configurationProvider,
            IInstrumentationClient instrumentation,
            IKeySecretProvider keySecretProvider, IApiClient apiClient, ICertificateReader certificateReader,
            IOpenBankingClientProfileRepository clientProfileRepository, IOpenBankingClientRepository clientRepository,
            ISoftwareStatementProfileRepository softwareStatementRepo, IEntityMapper entityMapper,
            IDomesticConsentRepository domesticConsentRepo)
        {
            CertificateReader = certificateReader.ArgNotNull(nameof(certificateReader));
            ApiClient = apiClient.ArgNotNull(nameof(ApiClient));
            EntityMapper = entityMapper.ArgNotNull(nameof(entityMapper));
            ConfigurationProvider = configurationProvider.ArgNotNull(nameof(configurationProvider));
            Instrumentation = instrumentation.ArgNotNull(nameof(instrumentation));
            KeySecretProvider = keySecretProvider.ArgNotNull(nameof(keySecretProvider));
            ClientProfileRepository = clientProfileRepository.ArgNotNull(nameof(clientProfileRepository));
            ClientRepository = clientRepository.ArgNotNull(nameof(clientRepository));
            SoftwareStatementRepository = softwareStatementRepo.ArgNotNull(nameof(softwareStatementRepo));
            DomesticConsentRepository = domesticConsentRepo.ArgNotNull(nameof(domesticConsentRepo));
        }

        public DateTimeOffset Created { get; set; }
        public ICertificateReader CertificateReader { get; }
        public IApiClient ApiClient { get; }
        public IConfigurationProvider ConfigurationProvider { get; }
        public IInstrumentationClient Instrumentation { get; }
        public IKeySecretProvider KeySecretProvider { get; }
        public IOpenBankingClientRepository ClientRepository { get; }
        public IOpenBankingClientProfileRepository ClientProfileRepository { get; }
        public ISoftwareStatementProfileRepository SoftwareStatementRepository { get; }
        public IDomesticConsentRepository DomesticConsentRepository { get; }
        public IEntityMapper EntityMapper { get; }
    }
}
