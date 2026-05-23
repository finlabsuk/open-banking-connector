// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Utility;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web.HostedServices;

public class WebAppInformationHostedService : IHostedService
{
    private readonly ApplicationPartManager _applicationPartManager;
    private readonly IConfigurationRoot _configurationRoot;
    private readonly EndpointDataSource _endpointDataSource;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILogger<WebAppInformationHostedService> _logger;
    private readonly string? _serviceVersion;

    public WebAppInformationHostedService(
        ILogger<WebAppInformationHostedService> logger,
        ApplicationPartManager applicationPartManager,
        EndpointDataSource endpointDataSource,
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHostApplicationLifetime hostApplicationLifetime,
        string? serviceVersion = null)
    {
        _logger = logger;
        _applicationPartManager = applicationPartManager;
        _endpointDataSource = endpointDataSource;
        _hostEnvironment = hostEnvironment;
        _hostApplicationLifetime = hostApplicationLifetime;
        _configurationRoot = (IConfigurationRoot) configuration;
        _serviceVersion = serviceVersion;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Log web app version
        if (_serviceVersion is not null)
        {
            _logger.LogInformation("Open Banking Connector Web App version: {Version}", _serviceVersion);
        }

        // Log operating system
        string osName = OsPlatformEnumHelper.GetCurrentOsPlatform() switch
        {
            OsPlatformEnum.MacOs => "macOS",
            OsPlatformEnum.Linux => "Linux",
            OsPlatformEnum.Windows => "Windows",
            _ => throw new ArgumentOutOfRangeException()
        };

        _logger.LogInformation("Operating system detected: {OsName}", osName);

        if (_hostEnvironment.IsDevelopment())
        {
            // Log application parts found
            IEnumerable<string> partNames = _applicationPartManager.ApplicationParts.Select(x => x.Name);
            _logger.LogInformation(string.Join(Environment.NewLine, partNames.Prepend("Application parts found:")));

            // Log controllers found
            var feature = new ControllerFeature();
            _applicationPartManager.PopulateFeature(feature);
            IEnumerable<string> controllerNames = feature.Controllers.Select(x => x.Name);
            _logger.LogInformation(string.Join(Environment.NewLine, controllerNames.Prepend("Controllers found:")));

            // Log endpoints found (once middleware is fully built)
            _hostApplicationLifetime.ApplicationStarted.Register(
                () =>
                {
                    IEnumerable<string?> endpointNames = _endpointDataSource.Endpoints.Select(x => x.DisplayName);
                    _logger.LogInformation(string.Join(Environment.NewLine, endpointNames.Prepend("Endpoints found:")));
                });
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
