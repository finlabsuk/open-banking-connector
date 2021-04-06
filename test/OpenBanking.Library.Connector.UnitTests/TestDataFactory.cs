﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using NSubstitute;
using SoftwareStatementProfileCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Repository.SoftwareStatementProfile;


namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests
{
    internal static class TestDataFactory
    {
        public static SharedContext CreateMockOpenBankingContext()
        {
            return new SharedContext(
                Substitute.For<ITimeProvider>(),
                Substitute.For<IApiClient>(),
                Substitute.For<IInstrumentationClient>(),
                Substitute.For<IDbService>(),
                Substitute
                    .For<IReadOnlyRepository<SoftwareStatementProfileCached>>(),
                Substitute.For<IApiVariantMapper>());
        }


        public static RequestBuilder CreateMockRequestBuilder()
        {
            return new RequestBuilder(
                Substitute.For<ITimeProvider>(),
                new ApiVariantMapper(),
                Substitute.For<IInstrumentationClient>(),
                Substitute.For<IApiClient>(),
                Substitute.For<IReadOnlyRepository<SoftwareStatementProfileCached>>(),
                Substitute.For<IDbService>());
        }
    }
}
