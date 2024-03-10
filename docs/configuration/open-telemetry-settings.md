# Open Telemetry settings

[Open Telemetry settings](#settings) configure support for Open Telemetry in Open Banking Connector.

Open Telemetry allows centralised collection of logging, distributed tracing and metrics data from running applications/services such as Open Banking Connector. The data format is standardised and vendor-neutral.

The service name used by Open Banking Connector for Open Telemetry can be customised using the setting `OpenBankingConnector:OpenTelemetry:ServiceName`. The service version used for Open Telemetry will be set to the product version of Open Banking Connector.

## Support for Open Telemetry logging

Support for Open Telemetry logging can be enabled by specifying an exporter URL in `OpenBankingConnector:OpenTelemetry:Logging:OtlpExporterUrl`.

By default, Open Telemetry trace and span IDs (related to tracing) are included with all logs including console logs.

## Support for Open Telemetry tracing

Support for Open Telemetry tracing can be enabled by setting `OpenBankingConnector:OpenTelemetry:UseConsoleExporter` to `true` (for console output) or specifying an exporter URL in `OpenBankingConnector:OpenTelemetry:Tracing:OtlpExporterUrl`. You can control the level of output by means of `OpenBankingConnector:OpenTelemetry:Tracing:ProviderFilter`.

 To propagate trace IDs from a service making requests to Open Banking Connector, please include the `traceparent` header as described [here](https://doordash.engineering/2021/06/17/leveraging-opentelemetry-for-custom-context-propagation/#:~:text=A%20close%20look%20at%20OpenTelemetry%E2%80%99s%20propagation%20formats) in requests to Open Banking Connector.

## Support for Open Telemetry metrics

Support for Open Telemetry metrics can be enabled by setting `OpenBankingConnector:OpenTelemetry:UseConsoleExporter` to `true` (for console output) or specifying an exporter URL in `OpenBankingConnector:OpenTelemetry:Metrics:OtlpExporterUrl`. You can control the reporting frequency and type of metrics export by means of `OpenBankingConnector:OpenTelemetry:Metrics:MetricReaderExportIntervalMilliseconds` and `OpenBankingConnector:OpenTelemetry:Metrics:MetricReaderTemporality`.

Currently only TPP Reporting metrics (see [here](https://www.openbanking.org.uk/jroc/#documents) for more info) are produced.

## Settings

| Name                                                                                                | Valid Values                | Default Value(s)                       | Description                                                                                                                                                     |
|-----------------------------------------------------------------------------------------------------|-----------------------------|----------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| OpenBankingConnector<wbr/>:OpenTelemetry<wbr/>:ServiceName                                          | string                      | `"OpenBankingConnector"`               | Use to customise the service name used with Open Telemetry                                                                                                      |
| OpenBankingConnector<wbr/>:OpenTelemetry<wbr/>:ServiceNamespace                                     | string                      | `"FinnovationLabs"`                    | Use to customise the service namespace used with Open Telemetry                                                                                                 |
| OpenBankingConnector<wbr/>:OpenTelemetry<wbr/>:ServiceInstanceId                                    | string                      | `"Default"`                            | Use to customise the service instance ID used with Open Telemetry                                                                                               |
| OpenBankingConnector<wbr/>:OpenTelemetry<wbr/>:UseConsoleExporter                                   | {`"true"`, `"false"`}       | `"false"`                              | Add console exporter to tracing and metrics. Will send tracing and metrics output to the console.                                                               |
| OpenBankingConnector<wbr/>:OpenTelemetry<wbr/>:Logging<wbr/>:OtlpExporterUrl                        | string                      | `""`                                   | When non-empty, add OTLP (Open Telemetry Protocol) exporter to logging with target URL as specified.                                                            |
| OpenBankingConnector<wbr/>:OpenTelemetry<wbr/>:Tracing<wbr/>:OtlpExporterUrl                        | string                      | `""`                                   | When non-empty, add OTLP (Open Telemetry Protocol) exporter to tracing with target URL as specified.                                                            |
| OpenBankingConnector<wbr/>:OpenTelemetry<wbr/>:Tracing<wbr/>:ProviderFilter                         | int                         | `"3"` (AspNetCore and HttpClient only) | Use to control tracing providers. Provide sum of desired providers where AspNetCore (top-level) = 1, HttpClient (bank requests) = 2, EFCore (database ORM) = 4. |
| OpenBankingConnector<wbr/>:OpenTelemetry<wbr/>:Metrics<wbr/>:OtlpExporterUrl                        | string                      | `""`                                   | When non-empty, add OTLP (Open Telemetry Protocol) exporter to metrics with target URL as specified.                                                            |
| OpenBankingConnector<wbr/>:OpenTelemetry<wbr/>:Metrics<wbr/>:MetricReaderExportIntervalMilliseconds | int                         | `"30000"`                              | Use to control frequency of export of metrics data (e.g. daily, hourly or more frequently when testing).                                                        |
| OpenBankingConnector<wbr/>:OpenTelemetry<wbr/>:Metrics<wbr/>:MetricReaderTemporality                | {`"Cumulative"`, `"Delta"`} | `"Cumulative"`                         | Use to control whether metric counters use cumulative or delta temporality for data export.                                                                     |

