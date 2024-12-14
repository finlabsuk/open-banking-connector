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
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.Extensions.Caching.Memory;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent;

internal class SharedContext : ISharedContext
{
    public SharedContext(
        ITimeProvider timeProvider,
        IApiClient apiClient,
        IInstrumentationClient instrumentation,
        IDbService dbService,
        IApiVariantMapper apiVariantMapper,
        IBankProfileService bankProfileService,
        IMemoryCache memoryCache,
        IEncryptionKeyDescription encryptionKeyInfo,
        ISecretProvider secretProvider,
        ISettingsProvider<HttpClientSettings> httpClientSettingsProvider,
        ObSealCertificateMethods obSealCertificateMethods,
        ObWacCertificateMethods obWacCertificateMethods,
        TppReportingMetrics tppReportingMetrics)
    {
        TimeProvider = timeProvider;
        ApiClient = apiClient;
        Instrumentation = instrumentation;
        DbService = dbService;
        ApiVariantMapper = apiVariantMapper;
        BankProfileService = bankProfileService;
        MemoryCache = memoryCache;
        EncryptionKeyInfo = encryptionKeyInfo;
        SecretProvider = secretProvider;
        HttpClientSettingsProvider = httpClientSettingsProvider;
        ObSealCertificateMethods = obSealCertificateMethods;
        ObWacCertificateMethods = obWacCertificateMethods;
        TppReportingMetrics = tppReportingMetrics;
    }

    public ITimeProvider TimeProvider { get; }
    public IMemoryCache MemoryCache { get; }
    public DateTimeOffset Created { get; set; }
    public IApiClient ApiClient { get; }
    public IInstrumentationClient Instrumentation { get; }
    public IBankProfileService BankProfileService { get; }
    public IDbService DbService { get; }
    public IEncryptionKeyDescription EncryptionKeyInfo { get; }
    public IApiVariantMapper ApiVariantMapper { get; }
    public ISecretProvider SecretProvider { get; }
    public ISettingsProvider<HttpClientSettings> HttpClientSettingsProvider { get; }

    public ObSealCertificateMethods ObSealCertificateMethods { get; }

    public ObWacCertificateMethods ObWacCertificateMethods { get; }

    public TppReportingMetrics TppReportingMetrics { get; }
}
