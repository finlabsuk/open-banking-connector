// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using Microsoft.Extensions.DependencyInjection;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost
{
    public interface IScopedRequestBuilder : IDisposable
    {
        IRequestBuilder RequestBuilder { get; }
    }

    public class ScopedRequestBuilder : IScopedRequestBuilder
    {
        private readonly IServiceScope _serviceScope;

        public ScopedRequestBuilder(IServiceProvider serviceProvider)
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
