// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;

/// <summary>
///     Settings for an app using Open Banking Connector which configure Configuration Sources.
/// </summary>
public class ConfigurationSourcesSettings : ISettings<ConfigurationSourcesSettings>
{
    /// <summary>
    ///     Use ASP.NET Core user secrets (intended for development purposes only).
    ///     This can be used to override default ASP.NET Core behaviour in which
    ///     user secrets are added in Development environment.
    /// </summary>
    public bool UseUserSecrets { get; set; } = false;

    /// <summary>
    ///     If non-empty, AWS parameter store secrets with given prefix are loaded and used.
    ///     For this to work, Open Banking Connector must be running in some environment
    ///     with an AWS IAM role/user having an appropriate permissions policy.
    /// </summary>
    public string AwsSsmParameterPrefix { get; set; } = string.Empty;

    public string SettingsGroupName => "OpenBankingConnector:ConfigurationSources";

    public ConfigurationSourcesSettings Validate() => this;
}
