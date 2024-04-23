// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent;

public class ManualServiceScope : IServiceScopeContainer
{
    private readonly BaseDbContext _dbContext;

    public ManualServiceScope(
        ITimeProvider timeProvider,
        IApiVariantMapper apiVariantMapper,
        IInstrumentationClient instrumentationClient,
        IApiClient apiClient,
        IEncryptionKeyInfo encryptionKeyInfo,
        DbContextOptions<SqliteDbContext> dbContextOptions,
        IBankProfileService bankProfileService,
        IMemoryCache memoryCache,
        ISecretProvider secretProvider,
        ISettingsProvider<HttpClientSettings> httpClientSettingsProvider,
        TppReportingMetrics ttpReportingMetrics)
    {
        _dbContext = new SqliteDbContext(dbContextOptions);
        DbService = new DbService(_dbContext);
        RequestBuilder = new RequestBuilder(
            timeProvider,
            apiVariantMapper,
            instrumentationClient,
            apiClient,
            DbService,
            bankProfileService,
            memoryCache,
            encryptionKeyInfo,
            secretProvider,
            httpClientSettingsProvider,
            ttpReportingMetrics);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public IRequestBuilder RequestBuilder { get; }

    public IDbService DbService { get; }
}
