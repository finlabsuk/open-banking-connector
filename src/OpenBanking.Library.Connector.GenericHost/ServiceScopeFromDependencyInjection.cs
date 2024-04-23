// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost;

/// <summary>
///     Scoped request builder source
/// </summary>
public class ServiceScopeFromDependencyInjection
    : IServiceScopeContainer
{
    private readonly IServiceScope _serviceScope;

    public ServiceScopeFromDependencyInjection(IServiceProvider serviceProvider)
    {
        _serviceScope = serviceProvider.CreateScope();
        DbService = _serviceScope.ServiceProvider.GetRequiredService<IDbService>();
        RequestBuilder = _serviceScope.ServiceProvider.GetRequiredService<IRequestBuilder>();
    }

    public IRequestBuilder RequestBuilder { get; }

    public IDbService DbService { get; }

    public void Dispose()
    {
        _serviceScope.Dispose();
    }
}
