// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;

[Flags]
public enum ProviderFilter
{
    None = 0,
    AspNetCore = 1,
    HttpClient = 2,
    EfCore = 4
}

public class TracingSettings
{
    public string OtlpExporterUrl { get; set; } = string.Empty;

    public bool UseConsoleExporter { get; set; } = false;

    public ProviderFilter ProviderFilter { get; set; } = ProviderFilter.AspNetCore | ProviderFilter.HttpClient;
}

/// <summary>
///     Settings for an Open Banking Connector app which configures Open Telemetry.
/// </summary>
public class OpenTelemetrySettings : ISettings<OpenTelemetrySettings>
{
    public string ServiceName { get; set; } = "FinnovationLabs.OpenBanking.Connector";

    public TracingSettings Tracing { get; set; } = new();

    public string SettingsGroupName => "OpenBankingConnector:OpenTelemetry";

    public OpenTelemetrySettings Validate() => this;
}
