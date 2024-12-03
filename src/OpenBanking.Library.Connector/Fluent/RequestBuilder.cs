// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Utility;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.Extensions.Caching.Memory;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent;

/// <summary>
///     Root object of Fluent Interface to Open Banking Connector
/// </summary>
public interface IRequestBuilder
{
    /// <summary>
    ///     API for setting up banks in Open Banking Connector including OAuth2 clients and functional APIs.
    /// </summary>
    IManagementContext Management { get; }

    /// <summary>
    ///     API corresponding to UK Open Banking Payment Initiation functional API.
    /// </summary>
    IPaymentInitiationContext PaymentInitiation { get; }

    /// <summary>
    ///     API corresponding to UK Open Banking Account and Transaction functional API.
    /// </summary>
    IAccountAndTransactionContext AccountAndTransaction { get; }

    /// <summary>
    ///     API corresponding to UK Open Banking Variable Recurring Payments functional API.
    /// </summary>
    IVariableRecurringPaymentsContext VariableRecurringPayments { get; }

    /// <summary>
    ///     API-independent methods for auth contexts. Method for passing back auth result obtained via bank redirect is
    ///     included.
    /// </summary>
    IAuthContextsContext AuthContexts { get; }

    /// <summary>
    ///     API containing utility methods.
    /// </summary>
    IUtilityContext Utility { get; }
}

public class RequestBuilder : IRequestBuilder
{
    private readonly IApiClient _apiClient;
    private readonly IApiVariantMapper _apiVariantMapper;
    private readonly IBankProfileService _bankProfileService;
    private readonly IDbService _dbService;
    private readonly EncryptionKeyDescriptionMethods _encryptionKeyDescriptionMethods;
    private readonly IEncryptionKeyInfo _encryptionKeyInfo;
    private readonly ISettingsProvider<HttpClientSettings> _httpClientSettingsProvider;
    private readonly IInstrumentationClient _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly ObSealCertificateMethods _obSealCertificateMethods;
    private readonly ObWacCertificateMethods _obWacCertificateMethods;
    private readonly ISecretProvider _secretProvider;
    private readonly ITimeProvider _timeProvider;
    private readonly TppReportingMetrics _tppReportingMetrics;

    public RequestBuilder(
        ITimeProvider timeProvider,
        IApiVariantMapper apiVariantMapper,
        IInstrumentationClient logger,
        IApiClient apiClient,
        IDbService dbService,
        IBankProfileService bankProfileService,
        IMemoryCache memoryCache,
        IEncryptionKeyInfo encryptionKeyInfo,
        ISecretProvider secretProvider,
        ISettingsProvider<HttpClientSettings> httpClientSettingsProvider,
        TppReportingMetrics tppReportingMetrics)
    {
        _timeProvider = timeProvider.ArgNotNull(nameof(timeProvider));
        _apiVariantMapper = apiVariantMapper.ArgNotNull(nameof(apiVariantMapper));
        _dbService = dbService;
        _bankProfileService = bankProfileService;
        _encryptionKeyInfo = encryptionKeyInfo;
        _secretProvider = secretProvider;
        _httpClientSettingsProvider = httpClientSettingsProvider;
        _tppReportingMetrics = tppReportingMetrics;
        _logger = logger.ArgNotNull(nameof(logger));
        _apiClient = apiClient.ArgNotNull(nameof(apiClient));
        _memoryCache = memoryCache.ArgNotNull(nameof(memoryCache));
        _obSealCertificateMethods = new ObSealCertificateMethods(
            memoryCache,
            secretProvider,
            _logger,
            dbService.GetDbEntityMethodsClass<ObSealCertificateEntity>());
        _obWacCertificateMethods = new ObWacCertificateMethods(
            httpClientSettingsProvider.GetSettings(),
            memoryCache,
            secretProvider,
            logger,
            tppReportingMetrics,
            dbService.GetDbEntityMethodsClass<ObWacCertificateEntity>());
        _encryptionKeyDescriptionMethods = new EncryptionKeyDescriptionMethods(
            memoryCache,
            secretProvider,
            logger,
            dbService.GetDbEntityMethodsClass<EncryptionKeyDescriptionEntity>());
    }

    public IManagementContext Management =>
        new ManagementContext(CreateContext());

    public IAccountAndTransactionContext AccountAndTransaction =>
        new AccountAndTransactionContext(CreateContext());

    public IPaymentInitiationContext PaymentInitiation =>
        new PaymentInitiationContext(CreateContext());

    public IVariableRecurringPaymentsContext VariableRecurringPayments =>
        new VariableRecurringPaymentsContext(CreateContext());

    public IAuthContextsContext AuthContexts =>
        new AuthContextsContext(CreateContext());

    public IUtilityContext Utility =>
        new UtilityContext(CreateContext());

    private ISharedContext CreateContext()
    {
        var context = new SharedContext(
            _timeProvider,
            _apiClient,
            _logger,
            _dbService,
            _apiVariantMapper,
            _bankProfileService,
            _memoryCache,
            _encryptionKeyInfo,
            _secretProvider,
            _httpClientSettingsProvider,
            _obSealCertificateMethods,
            _obWacCertificateMethods,
            _tppReportingMetrics,
            _encryptionKeyDescriptionMethods) { Created = _timeProvider.GetUtcNow() };
        return context;
    }
}
