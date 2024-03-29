// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.Extensions.Caching.Memory;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent;

public interface IRequestBuilderContainer : IDisposable
{
    IRequestBuilder RequestBuilder { get; }
}

public class RequestBuilderContainer : IRequestBuilderContainer
{
    private readonly BaseDbContext _dbContext;

    public RequestBuilderContainer(
        ITimeProvider timeProvider,
        IApiVariantMapper apiVariantMapper,
        IInstrumentationClient instrumentationClient,
        IApiClient apiClient,
        IProcessedSoftwareStatementProfileStore softwareStatementProfilesRepository,
        IEncryptionKeyInfo encryptionKeyInfo,
        BaseDbContext dbContext,
        IBankProfileService bankProfileService,
        IMemoryCache memoryCache)
    {
        _dbContext = dbContext;
        RequestBuilder = new RequestBuilder(
            timeProvider,
            apiVariantMapper,
            instrumentationClient,
            apiClient,
            softwareStatementProfilesRepository,
            new DbService(dbContext),
            bankProfileService,
            memoryCache,
            encryptionKeyInfo);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public IRequestBuilder RequestBuilder { get; }
}
