// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Access;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using SoftwareStatementProfileCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets.Cached.SoftwareStatementProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public interface ISharedContext
    {
        DateTimeOffset Created { get; set; }
        ICertificateReader CertificateReader { get; }
        IApiClient ApiClient { get; }
        IInstrumentationClient Instrumentation { get; }
        IKeySecretReadOnlyProvider KeySecretReadOnlyProvider { get; }
        IDbMultiEntityMethods DbContextService { get; }
        IDbEntityRepositoryFactory DbEntityRepositoryFactory { get; }
        IReadOnlyKeySecretItemRepository<SoftwareStatementProfileCached> SoftwareStatementProfileCachedRepo { get; }
        IEntityMapper EntityMapper { get; }
        ITimeProvider TimeProvider { get; }
    }
}
