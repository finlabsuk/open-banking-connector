// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.Extensions.Hosting;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.HostedServices
{
    public class SoftwareStatementProfileHostedService : IHostedService
    {
        private readonly ISoftwareStatementProfileService _softwareStatementProfileService;

        public SoftwareStatementProfileHostedService(ISoftwareStatementProfileService softwareStatementProfileService)
        {
            _softwareStatementProfileService = softwareStatementProfileService ??
                                               throw new ArgumentNullException(nameof(softwareStatementProfileService));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _softwareStatementProfileService.SetSoftwareStatementProfileFromSecrets();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
