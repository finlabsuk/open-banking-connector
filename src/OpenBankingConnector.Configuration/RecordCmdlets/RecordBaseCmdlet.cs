// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Net.Http;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace OpenBankingConnector.Configuration.RecordCmdlets
{
    public class RecordBaseCmdlet : BaseCmdlet, IDisposable
    {
        protected readonly ServiceProvider _serviceProvider;
        protected readonly MemoryStream _memoryStream;
        protected readonly StreamWriter _streamWriter;

        public RecordBaseCmdlet(string verbName, string nounName) : base(verbName: verbName, nounName: nounName)
        {
            // Register services
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IKeySecretProvider, MemoryKeySecretProvider>();
            const string httpClientName = "OBC";

            //  Simplify to new ApiClient(new HttpClient()?
            serviceCollection.AddHttpClient(httpClientName)
                .ConfigurePrimaryHttpMessageHandler(
                    sp =>
                    {
                        IKeySecretProvider secrets = sp.GetService<IKeySecretProvider>();

                        HttpMessageHandler handler = new HttpRequestBuilder()
                            .SetClientCertificates(CertificateFactories.GetCertificates(secrets))
                            .CreateMessageHandler();

                        return handler;
                    });
            serviceCollection.AddSingleton<IConfigurationProvider, DefaultConfigurationProvider>();

            _memoryStream = new MemoryStream();
            _streamWriter = new StreamWriter(_memoryStream, Encoding.UTF8);
            serviceCollection.AddSingleton<IInstrumentationClient, ConsoleInstrumentationClient>(
                s => new ConsoleInstrumentationClient(_streamWriter));
            serviceCollection.AddSingleton<IApiClient>(
                sp =>
                {
                    IHttpClientFactory hcf = sp.GetService<IHttpClientFactory>();

                    HttpClient client = hcf.CreateClient(httpClientName);

                    return new ApiClient(instrumentation: sp.GetService<IInstrumentationClient>(), httpClient: client);
                });
            serviceCollection.AddSingleton<ICertificateReader, PemParsingCertificateReader>();
            serviceCollection.AddSingleton<IEntityMapper, EntityMapper>();

            // Configure DB
            serviceCollection
                // See e.g. https://jasonwatmore.com/post/2020/01/03/aspnet-core-ef-core-migrations-for-multiple-databases-sqlite-and-sql-server 
                .AddDbContext<BaseDbContext, SqliteDbContext>(
                    optionsAction: options =>
                    {
                        string connectionString =
                            "Data Source=C:/Repos/GitHub/markm77/open-banking-connector-csharp/src/OpenBanking.Library.Connector/sqliteTestDb.db";
                        options.UseSqlite(connectionString);
                    },
                    contextLifetime: ServiceLifetime.Singleton,
                    optionsLifetime: ServiceLifetime.Singleton);

            serviceCollection.AddScoped<IDbMultiEntityMethods,
                DbMultiEntityMethods>();
            serviceCollection.AddScoped<IDbEntityRepository<SoftwareStatementProfile>,
                DbEntityRepository<SoftwareStatementProfile>>();
            serviceCollection.AddScoped<IDbEntityRepository<BankClientProfile>,
                DbEntityRepository<BankClientProfile>>();
            serviceCollection.AddScoped<IDbEntityRepository<ApiProfile>,
                DbEntityRepository<ApiProfile>>();
            serviceCollection.AddScoped<IDbEntityRepository<DomesticConsent>,
                DbEntityRepository<DomesticConsent>>();

            serviceCollection.AddScoped<ICreateSoftwareStatementProfile, CreateSoftwareStatementProfile>();
            serviceCollection.AddScoped<ICreateBankClientProfile, CreateBankClientProfile>();

            // Create service provider
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public void Dispose()
        {
            _memoryStream.Dispose();
        }
    }
}
