# Open Telemetry settings

[Open Telemetry settings](#settings) configure support for Open Telemetry in Open Banking Connector.

Open Telemetry is a system for collecting logging, tracing and metrics output from an application (service) like Open Banking Connector. Open Banking Connector support for each of these is indicated below.

The service name used by Open Banking Connector for Open Telemetry can be customised using the setting `OpenBankingConnector:OpenTelemetry:ServiceName`. The service version used for Open Telemetry will be set to the product version of Open Banking Connector.

## Support for Open Telemetry logging

By default, Open Telemetry trace and span IDs are included with logs. To propagate trace IDs from a service making requests to Open Banking Connector, please include the `traceparent` header as described [here](https://doordash.engineering/2021/06/17/leveraging-opentelemetry-for-custom-context-propagation/#:~:text=A%20close%20look%20at%20OpenTelemetry%E2%80%99s%20propagation%20formats) in requests.

## Support for Open Telemetry tracing

Support for Open Telemetry tracing can be enabled by setting `OpenBankingConnector:OpenTelemetry:Tracing:UseConsoleExporter` to true (for console output) or specifying an exporter URL in `OpenBankingConnector:OpenTelemetry:Tracing:OtlpExporterUrl`. You can control the level of output by means of `OpenBankingConnector:OpenTelemetry:Tracing:ProviderFilter`.

## Support for Open Telemetry metrics

This is not enabled yet.

## Settings

| Name                                                                            | Valid Values | Default Value(s)                          | Description                                                                                                                                                     |
|---------------------------------------------------------------------------------|--------------|-------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| OpenBankingConnector<wbr/>:OpenTelemetry<wbr/>:ServiceName                      | string       | `"FinnovationLabs.OpenBanking.Connector"` | Use to customise the service name used with Open Telemetry                                                                                                      |
| OpenBankingConnector<wbr/>:OpenTelemetry<wbr/>:Tracing<wbr/>:UseConsoleExporter | bool         | `false`                                   | Add console exporter to tracing. Will send tracing output to the console.                                                                                       |
| OpenBankingConnector<wbr/>:OpenTelemetry<wbr/>:Tracing<wbr/>:OtlpExporterUrl    | string       | `""`                                      | When non-empty, add OTLP (Open Telemetry Protocol) exporter to tracing with target URL as specified.                                                            |
| OpenBankingConnector<wbr/>:OpenTelemetry<wbr/>:Tracing<wbr/>:ProviderFilter     | int          | `3` (AspNetCore and HttpClient only)      | Use to control tracing providers. Provide sum of desired providers where AspNetCore (top-level) = 1, HttpClient (bank requests) = 2, EFCore (database ORM) = 4. |

