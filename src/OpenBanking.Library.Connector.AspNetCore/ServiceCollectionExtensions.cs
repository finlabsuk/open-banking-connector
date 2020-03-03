// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IConfigurationProvider = FinnovationLabs.OpenBanking.Library.Connector.Configuration.IConfigurationProvider;

namespace FinnovationLabs.OpenBanking.Library.Connector.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenBankingConnector(this IServiceCollection services,
            IConfiguration config)
        {
            const string httpClientName = "OBC.NET";

            services.AddHttpClient(httpClientName)
                .ConfigurePrimaryHttpMessageHandler(sp =>
                {
                    var secrets = sp.GetService<IKeySecretProvider>();

                    var handler = new HttpRequestBuilder()
                        .SetClientCertificates(CertificateFactories.GetCertificates(secrets))
                        .CreateMessageHandler();

                    return handler;
                });

            services.AddSingleton<IConfigurationProvider>(sp => new AppsettingsConfigurationProvider(config));
            services.AddSingleton<IInstrumentationClient, InstrumentationClient>();
            services.AddSingleton(sp =>
            {
                var builder = new KeySecretBuilder();
                var configProvider = sp.GetService<IConfigurationProvider>();

                return builder.GetKeySecretProvider(config, configProvider.GetRuntimeConfiguration());
            });
            services.AddSingleton<ICertificateReader, PemParsingCertificateReader>();
            services.AddSingleton<ISoftwareStatementProfileRepository, MemorySoftwareStatementProfileRepository>();
            services.AddSingleton<IDomesticConsentRepository, MemoryDomesticConsentRepository>();
            services.AddSingleton<IOpenBankingClientProfileRepository, MemoryOpenBankingClientProfileRepository>();
            services.AddSingleton<IEntityMapper, EntityMapper>();
            services.AddTransient<IOpenBankingRequestBuilder, OpenBankingRequestBuilder>();
            services.AddSingleton<IApiClient>(sp =>
            {
                var hcf = sp.GetService<IHttpClientFactory>();

                var client = hcf.CreateClient(httpClientName);

                return new ApiClient(sp.GetService<IInstrumentationClient>(), client);
            });

            return services;
        }
    }
}
