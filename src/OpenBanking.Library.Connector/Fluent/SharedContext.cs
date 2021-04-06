﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using SoftwareStatementProfileCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Repository.SoftwareStatementProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    internal class SharedContext : ISharedContext
    {
        public SharedContext(
            ITimeProvider timeProvider,
            IApiClient apiClient,
            IInstrumentationClient instrumentation,
            IDbService dbService,
            IReadOnlyRepository<SoftwareStatementProfileCached> softwareStatementProfileCachedRepo,
            IApiVariantMapper apiVariantMapper)
        {
            TimeProvider = timeProvider;
            ApiClient = apiClient;
            Instrumentation = instrumentation;
            DbService = dbService;
            SoftwareStatementProfileCachedRepo = softwareStatementProfileCachedRepo;
            ApiVariantMapper = apiVariantMapper;
        }

        public ITimeProvider TimeProvider { get; }
        public DateTimeOffset Created { get; set; }
        public IApiClient ApiClient { get; }
        public IInstrumentationClient Instrumentation { get; }
        public IDbService DbService { get; }
        public IReadOnlyRepository<SoftwareStatementProfileCached> SoftwareStatementProfileCachedRepo { get; }
        public IApiVariantMapper ApiVariantMapper { get; }
    }
}
