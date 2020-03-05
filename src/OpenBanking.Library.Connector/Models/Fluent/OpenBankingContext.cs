// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Security.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public class OpenBankingContext
    {
        internal OpenBankingContext(IConfigurationProvider configurationProvider,
            IInstrumentationClient instrumentation,
            IKeySecretProvider keySecretProvider, IApiClient apiClient, ICertificateReader certificateReader,
            IOpenBankingClientProfileRepository clientProfileRepository,
            ISoftwareStatementProfileRepository softwareStatementRepo, IEntityMapper entityMapper,
            IDomesticConsentRepository domesticConsentRepo, IApiProfileRepository apiProfileRepository)
        {
            CertificateReader = certificateReader.ArgNotNull(nameof(certificateReader));
            ApiClient = apiClient.ArgNotNull(nameof(ApiClient));
            EntityMapper = entityMapper.ArgNotNull(nameof(entityMapper));
            ConfigurationProvider = configurationProvider.ArgNotNull(nameof(configurationProvider));
            Instrumentation = instrumentation.ArgNotNull(nameof(instrumentation));
            KeySecretProvider = keySecretProvider.ArgNotNull(nameof(keySecretProvider));
            ClientProfileRepository = clientProfileRepository.ArgNotNull(nameof(clientProfileRepository));
            SoftwareStatementRepository = softwareStatementRepo.ArgNotNull(nameof(softwareStatementRepo));
            DomesticConsentRepository = domesticConsentRepo.ArgNotNull(nameof(domesticConsentRepo));
            ApiProfileRepository = apiProfileRepository.ArgNotNull(nameof(apiProfileRepository));
        }

        public DateTimeOffset Created { get; set; }
        public ICertificateReader CertificateReader { get; }
        public IApiClient ApiClient { get; }
        public IConfigurationProvider ConfigurationProvider { get; }
        public IInstrumentationClient Instrumentation { get; }
        public IKeySecretProvider KeySecretProvider { get; }
        public IOpenBankingClientProfileRepository ClientProfileRepository { get; }
        public ISoftwareStatementProfileRepository SoftwareStatementRepository { get; }
        public IDomesticConsentRepository DomesticConsentRepository { get; }
        public IEntityMapper EntityMapper { get; }

        public IApiProfileRepository ApiProfileRepository { get; }
    }
}
