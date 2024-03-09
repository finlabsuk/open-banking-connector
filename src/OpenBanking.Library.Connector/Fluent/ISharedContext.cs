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
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.Extensions.Caching.Memory;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent;

public interface ISharedContext
{
    DateTimeOffset Created { get; set; }
    IApiClient ApiClient { get; }
    IInstrumentationClient Instrumentation { get; }

    IBankProfileService BankProfileService { get; }
    IDbService DbService { get; }
    
    IEncryptionKeyInfo EncryptionKeyInfo { get; }

    IApiVariantMapper ApiVariantMapper { get; }
    ITimeProvider TimeProvider { get; }

    IMemoryCache MemoryCache { get; }
    ISecretProvider SecretProvider { get; }
    ISettingsProvider<HttpClientSettings> HttpClientSettingsProvider { get; }

    ObSealCertificateMethods ObSealCertificateMethods { get; }

    ObWacCertificateMethods ObWacCertificateMethods { get; }
    TppReportingMetrics TppReportingMetrics { get; }
}
