// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using Microsoft.Extensions.Options;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Configuration;

/// <summary>
///     Settings provider that gets settings via ISettings from .NET Generic Host configuration
/// </summary>
/// <typeparam name="TSettings"></typeparam>
public class ConfigurationSettingsProvider<TSettings> : ISettingsProvider<TSettings>
    where TSettings : class, ISettings<TSettings>, new()
{
    private readonly TSettings _settings;

    public ConfigurationSettingsProvider(IOptions<TSettings> options)
    {
        _settings = options.Value;
    }

    public TSettings GetSettings() => _settings;
}
