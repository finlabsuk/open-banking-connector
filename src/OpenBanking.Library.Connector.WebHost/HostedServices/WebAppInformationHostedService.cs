// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FinnovationLabs.OpenBanking.Library.Connector.WebHost.HostedServices
{
    public class WebAppInformationHostedService : IHostedService
    {
        private readonly ApplicationPartManager _applicationPartManager;
        private readonly EndpointDataSource _endpointDataSource;
        private readonly ILogger<WebAppInformationHostedService> _logger;

        public WebAppInformationHostedService(
            ILogger<WebAppInformationHostedService> logger,
            ApplicationPartManager applicationPartManager,
            EndpointDataSource endpointDataSource)
        {
            _logger = logger;
            _applicationPartManager = applicationPartManager;
            _endpointDataSource = endpointDataSource;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Log application parts found
            IEnumerable<string> partNames = _applicationPartManager.ApplicationParts.Select(x => x.Name);
            _logger.LogInformation(
                string.Join(separator: Environment.NewLine, values: partNames.Prepend("Application parts found:")));

            // Log controllers found
            ControllerFeature feature = new ControllerFeature();
            _applicationPartManager.PopulateFeature(feature);
            IEnumerable<string> controllerNames = feature.Controllers.Select(x => x.Name);
            _logger.LogInformation(
                string.Join(separator: Environment.NewLine, values: controllerNames.Prepend("Controllers found:")));

            // Log endpoints found
            Task.Run(
                async () =>
                {
                    await Task.Delay(3000); // wait 3 seconds for middleware to be built
                    IEnumerable<string> endpointNames = _endpointDataSource.Endpoints.Select(x => x.DisplayName);
                    _logger.LogInformation(
                        string.Join(separator: Environment.NewLine, values: endpointNames.Prepend("Endpoints found:")));
                });
    
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
