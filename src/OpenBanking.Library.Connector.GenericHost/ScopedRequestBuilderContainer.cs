// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using Microsoft.Extensions.DependencyInjection;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost
{
    /// <summary>
    ///     Scoped request builder source
    /// </summary>
    public class ScopedRequestBuilderContainer : IRequestBuilderContainer
    {
        private readonly IServiceScope _serviceScope;

        public ScopedRequestBuilderContainer(IServiceProvider serviceProvider)
        {
            _serviceScope = serviceProvider.CreateScope();
            RequestBuilder = _serviceScope.ServiceProvider.GetRequiredService<IRequestBuilder>();
        }

        public IRequestBuilder RequestBuilder { get; }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}
