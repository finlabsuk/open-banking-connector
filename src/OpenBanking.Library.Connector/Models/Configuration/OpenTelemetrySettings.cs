// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using OpenTelemetry.Metrics;

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

    public ProviderFilter ProviderFilter { get; set; } = ProviderFilter.AspNetCore | ProviderFilter.HttpClient;
}

public class LoggingSettings
{
    public string OtlpExporterUrl { get; set; } = string.Empty;
}

/// <summary>
///     Settings for an Open Banking Connector app which configures Open Telemetry.
/// </summary>
public class OpenTelemetrySettings : ISettings<OpenTelemetrySettings>
{
    public string ServiceName { get; set; } = "OpenBankingConnector";

    public string ServiceNamespace { get; set; } = "FinnovationLabs";

    public string ServiceInstanceId { get; set; } = "Default";

    public bool UseConsoleExporter { get; set; } = false;

    public LoggingSettings Logging { get; set; } = new();

    public TracingSettings Tracing { get; set; } = new();

    public MetricsSettings Metrics { get; set; } = new();

    public string SettingsGroupName => "OpenBankingConnector:OpenTelemetry";

    public OpenTelemetrySettings Validate() => this;
}

public class MetricsSettings
{
    public string OtlpExporterUrl { get; set; } = string.Empty;

    public int MetricReaderExportIntervalMilliseconds { get; set; } = 30000;

    public MetricReaderTemporalityPreference MetricReaderTemporality { get; set; } =
        MetricReaderTemporalityPreference.Cumulative;
}
