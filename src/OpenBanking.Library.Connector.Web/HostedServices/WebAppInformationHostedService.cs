// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web.HostedServices
{
    public class WebAppInformationHostedService : IHostedService
    {
        private readonly ApplicationPartManager _applicationPartManager;
        private readonly IConfigurationRoot _configurationRoot;
        private readonly EndpointDataSource _endpointDataSource;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<WebAppInformationHostedService> _logger;

        public WebAppInformationHostedService(
            ILogger<WebAppInformationHostedService> logger,
            ApplicationPartManager applicationPartManager,
            EndpointDataSource endpointDataSource,
            IConfiguration configuration,
            IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _applicationPartManager = applicationPartManager;
            _endpointDataSource = endpointDataSource;
            _hostEnvironment = hostEnvironment;
            _configurationRoot = (IConfigurationRoot) configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_hostEnvironment.IsDevelopment())
            {
                // Log configuration
                _logger.LogInformation("Configuration:\n" + _configurationRoot.GetDebugView());

                // Log application parts found
                IEnumerable<string> partNames = _applicationPartManager.ApplicationParts.Select(x => x.Name);
                _logger.LogInformation(string.Join(Environment.NewLine, partNames.Prepend("Application parts found:")));

                // Log controllers found
                var feature = new ControllerFeature();
                _applicationPartManager.PopulateFeature(feature);
                IEnumerable<string> controllerNames = feature.Controllers.Select(x => x.Name);
                _logger.LogInformation(string.Join(Environment.NewLine, controllerNames.Prepend("Controllers found:")));

                // Log endpoints found
                Task.Run(
                    async () =>
                    {
                        await Task.Delay(3000); // wait 3 seconds for middleware to be built
                        IEnumerable<string?> endpointNames = _endpointDataSource.Endpoints.Select(x => x.DisplayName);
                        _logger.LogInformation(
                            string.Join(Environment.NewLine, endpointNames.Prepend("Endpoints found:")));
                    });
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
