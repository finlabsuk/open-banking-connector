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
    public interface ISharedContext
    {
        DateTimeOffset Created { get; set; }
        IApiClient ApiClient { get; }
        IInstrumentationClient Instrumentation { get; }
        IDbService DbService { get; }
        IReadOnlyRepository<SoftwareStatementProfileCached> SoftwareStatementProfileCachedRepo { get; }
        IApiVariantMapper ApiVariantMapper { get; }
        ITimeProvider TimeProvider { get; }
    }
}
