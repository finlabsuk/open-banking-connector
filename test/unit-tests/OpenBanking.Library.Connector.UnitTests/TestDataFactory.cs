// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Security.PaymentInitiation;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests
{
    internal static class TestDataFactory
    {
        public static SharedContext CreateMockOpenBankingContext()
        {
            return new SharedContext(
                new SqliteDbContext(new DbContextOptions<SqliteDbContext>()),
                Substitute.For<ICertificateReader>(),
                Substitute.For<IApiClient>(),
                Substitute.For<IConfigurationProvider>(),
                Substitute.For<IInstrumentationClient>(),
                Substitute.For<IKeySecretProvider>(),
                Substitute.For<IOpenBankingClientProfileRepository>(),
                Substitute.For<ISoftwareStatementProfileRepository>(),
                Substitute.For<IDomesticConsentRepository>(),
                Substitute.For<IEntityMapper>(),
                Substitute.For<IApiProfileRepository>()
            );
        }


        public static RequestBuilder CreateMockRequestBuilder()
        {
            return new RequestBuilder(
                Substitute.For<ITimeProvider>(),
                new EntityMapper(),
                new SqliteDbContext(new DbContextOptions<SqliteDbContext>()), 
                new DefaultConfigurationProvider(),
                Substitute.For<IInstrumentationClient>(),
                Substitute.For<IKeySecretProvider>(),
                Substitute.For<IApiClient>(),
                Substitute.For<ICertificateReader>(),
                Substitute.For<IOpenBankingClientProfileRepository>(),
                Substitute.For<ISoftwareStatementProfileRepository>(),
                Substitute.For<IDomesticConsentRepository>(),
                Substitute.For<IApiProfileRepository>());
        }
    }
}
