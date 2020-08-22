﻿// Licensed to Finnovation Labs Limited under one or more agreements.
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
    public class SharedContext : ISharedContext
    {
        internal SharedContext(
            ICertificateReader certificateReader,
            IApiClient apiClient,
            IObcConfigurationProvider configurationProvider,
            IInstrumentationClient instrumentation,
            IKeySecretReadOnlyProvider keySecretReadOnlyProvider,
            IDbEntityRepository<BankRegistration> clientProfileRepository,
            ISoftwareStatementProfileService softwareStatementProfileService,
            IDbEntityRepository<DomesticPaymentConsent> domesticConsentRepository,
            IEntityMapper entityMapper,
            IDbEntityRepository<BankProfile> apiProfileRepository,
            IDbMultiEntityMethods dbContextService,
            IKeySecretWriteRepository<ActiveSoftwareStatementProfiles> activeSrRepo,
            IKeySecretMultiItemReadRepository<SoftwareStatementProfile> sReadOnlyRepo,
            IKeySecretMultiItemWriteRepository<SoftwareStatementProfile> sRepo,
            IKeySecretReadRepository<ActiveSoftwareStatementProfiles> activeSReadOnlyRepo,
            IDbEntityRepository<Bank> bankRepository)
        {
            CertificateReader = certificateReader;
            ApiClient = apiClient;
            ConfigurationProvider = configurationProvider;
            Instrumentation = instrumentation;
            KeySecretReadOnlyProvider = keySecretReadOnlyProvider;
            BankRegistrationRepository = clientProfileRepository;
            SoftwareStatementProfileService = softwareStatementProfileService;
            DomesticConsentRepository = domesticConsentRepository;
            EntityMapper = entityMapper;
            BankProfileRepository = apiProfileRepository;
            DbContextService = dbContextService;
            ActiveSRRepo = activeSrRepo;
            SReadOnlyRepo = sReadOnlyRepo;
            SRepo = sRepo;
            ActiveSReadOnlyRepo = activeSReadOnlyRepo;
            BankRepository = bankRepository;
        }

        public DateTimeOffset Created { get; set; }
        public ICertificateReader CertificateReader { get; }
        public IApiClient ApiClient { get; }
        public IObcConfigurationProvider ConfigurationProvider { get; }
        public IInstrumentationClient Instrumentation { get; }
        public IKeySecretReadOnlyProvider KeySecretReadOnlyProvider { get; }
        public IDbEntityRepository<Bank> BankRepository { get; }
        public IDbEntityRepository<BankRegistration> BankRegistrationRepository { get; }
        public IKeySecretWriteRepository<ActiveSoftwareStatementProfiles> ActiveSRRepo { get; }
        public IKeySecretMultiItemReadRepository<SoftwareStatementProfile> SReadOnlyRepo { get; }
        public IKeySecretMultiItemWriteRepository<SoftwareStatementProfile> SRepo { get; }
        public IKeySecretReadRepository<ActiveSoftwareStatementProfiles> ActiveSReadOnlyRepo { get; }
        public ISoftwareStatementProfileService SoftwareStatementProfileService { get; }
        public IDbEntityRepository<DomesticPaymentConsent> DomesticConsentRepository { get; }
        public IEntityMapper EntityMapper { get; }
        public IDbEntityRepository<BankProfile> BankProfileRepository { get; }
        public IDbMultiEntityMethods DbContextService { get; }
    }
}
