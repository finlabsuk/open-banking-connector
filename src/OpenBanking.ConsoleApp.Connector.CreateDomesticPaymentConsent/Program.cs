// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FinnovationLabs.OpenBanking.ConsoleApp.Connector.CreateDomesticPaymentConsent
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Create and start .NET Generic Host
            using IHost host = CreateHostBuilder(args)
                .Build();
            host.Start();

            // Perform operations using Open Banking Connector
            PerformOperations(host)
                .GetAwaiter()
                .GetResult();
        }

        private static async Task PerformOperations(IHost host)
        {
            // Get bank profile definitions
            var bankProfileDefinitions =
                host.Services.GetRequiredService<BankProfileDefinitions>();

            // Get scoped request builder
            using IScopedRequestBuilder scopedRequestBuilder =
                new ScopedRequestBuilder(host.Services);
            IRequestBuilder requestBuilder = scopedRequestBuilder.RequestBuilder;

            // Create bank configuration
            // (includes Bank, BankRegistration and BankApiInformation)
            BankProfile bankProfile = bankProfileDefinitions.Modelo;
            string demoNameUnique = "Demo" + Guid.NewGuid();
            (Guid bankId, Guid bankRegistrationId, Guid bankApiInformationId) =
                await BankConfigurationMethods.Create(
                    "All",
                    RegistrationScope.All,
                    requestBuilder,
                    bankProfileDefinitions.Modelo,
                    demoNameUnique);

            // Create domestic payment consent
            Guid domesticPaymentConsentId =
                await DomesticPaymentConsentMethods.Create(
                    bankProfile,
                    bankRegistrationId,
                    bankApiInformationId,
                    requestBuilder,
                    demoNameUnique);

            // Retrieve domestic payment consent
            await DomesticPaymentConsentMethods.Get(requestBuilder, domesticPaymentConsentId);
        }

        // Create host builder method based on default but with
        // service setup
        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(
                    (hostBuilderContext, services) =>
                        services
                            // Add .NET generic host app services 
                            .AddGenericHostServices(hostBuilderContext.Configuration));
    }
}
