// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost
{
    public class Helpers
    {
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            args ??= new string[0];

            IHostBuilder host = Host.CreateDefaultBuilder(args);
            bool useLocalSecrets = true; // TODO: In future, set this only when MemorySecretProvider configured

            // Add local secrets for non-development environments if necessary
            // ( CreateDefaultBuilder() only adds them for development environments. )
            if (useLocalSecrets)
            {
                host.ConfigureAppConfiguration(
                    (hostingContext, config) =>
                    {
                        if (!hostingContext.HostingEnvironment.IsDevelopment())
                        {
                            config.AddUserSecrets<Helpers>();
                        }
                    });
            }

            host.ConfigureServices(
                (hostContext, services) =>
                {
                    services.AddOpenBankingConnector(
                        configuration: hostContext.Configuration,
                        loadSecretsFromConfig: useLocalSecrets);
                    services.AddScoped<ICreateBankClientProfile, CreateBankClientProfile>();
                    services.AddScoped<ICreatePaymentInitiationApiProfile, CreatePaymentInitiationApiProfile>();
                });

            // Ensure "Development" is default environment unless otherwise specified.
            string preferredEnvironment =
                Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                "Development";
            host.UseEnvironment(preferredEnvironment);

            return host;
        }
    }
}
