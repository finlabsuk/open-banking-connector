// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Security.PaymentInitiation;
using NSubstitute;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests
{
    internal static class TestDataFactory
    {
        public static OpenBankingContext CreateMockOpenBankingContext()
        {
            return new OpenBankingContext(
                Substitute.For<IConfigurationProvider>(),
                Substitute.For<IInstrumentationClient>(),
                Substitute.For<IKeySecretProvider>(),
                Substitute.For<IApiClient>(),
                Substitute.For<ICertificateReader>(),
                Substitute.For<IOpenBankingClientProfileRepository>(),
                Substitute.For<ISoftwareStatementProfileRepository>(),
                Substitute.For<IEntityMapper>(),
                Substitute.For<IDomesticConsentRepository>(),
                Substitute.For<IApiProfileRepository>()
            );
        }


        public static OpenBankingRequestBuilder CreateMockRequestBuilder()
        {
            return new OpenBankingRequestBuilder(
                Substitute.For<ITimeProvider>(),
                new EntityMapper(),
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
