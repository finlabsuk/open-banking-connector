// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
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
            using IRequestBuilderContainer scopedRequestBuilder =
                new ScopedRequestBuilderContainer(host.Services);
            IRequestBuilder requestBuilder = scopedRequestBuilder.RequestBuilder;

            // Create bank configuration
            BankProfile bankProfile = bankProfileDefinitions.Modelo;
            string demoNameUnique = "Demo" + Guid.NewGuid();
            (Guid bankId, Guid bankRegistrationId) =
                await BankConfigurationMethods.Create(
                    "All",
                    null,
                    RegistrationScopeEnum.All,
                    requestBuilder,
                    bankProfileDefinitions.Modelo,
                    demoNameUnique);

            // Create bank API information
            PaymentInitiationApiRequest paymentInitiationApiRequest =
                bankProfile.GetPaymentInitiationApiRequest(bankId);
            paymentInitiationApiRequest.Name = demoNameUnique;
            IFluentResponse<PaymentInitiationApiResponse> response = await requestBuilder
                .BankConfiguration
                .PaymentInitiationApis
                .CreateLocalAsync(paymentInitiationApiRequest);
            Guid bankApiSetId = response.Data!.Id;


            // Create domestic payment consent
            Guid domesticPaymentConsentId =
                await DomesticPaymentConsentMethods.Create(
                    bankProfile,
                    bankRegistrationId,
                    bankApiSetId,
                    DomesticPaymentTypeEnum.PersonToMerchant,
                    Guid.NewGuid().ToString("N"),
                    Guid.NewGuid().ToString("N"),
                    requestBuilder,
                    demoNameUnique);

            // Read domestic payment consent
            await DomesticPaymentConsentMethods.Read(requestBuilder, domesticPaymentConsentId);
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
