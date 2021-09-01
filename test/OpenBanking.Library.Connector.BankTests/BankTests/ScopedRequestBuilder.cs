// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using SoftwareStatementProfileCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Repository.SoftwareStatementProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests
{
    public class ScopedRequestBuilder2 : IScopedRequestBuilder
    {
        private readonly BaseDbContext _dbContext;

        public ScopedRequestBuilder2(
            ITimeProvider timeProvider,
            IApiVariantMapper apiVariantMapper,
            IInstrumentationClient instrumentationClient,
            IApiClient apiClient,
            IReadOnlyRepository<SoftwareStatementProfileCached> softwareStatementProfilesRepository,
            BaseDbContext dbContext)
        {
            _dbContext = dbContext;
            RequestBuilder = new RequestBuilder(
                timeProvider,
                apiVariantMapper,
                instrumentationClient,
                apiClient,
                softwareStatementProfilesRepository,
                new DbService(dbContext));
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public IRequestBuilder RequestBuilder { get; }
    }
}
