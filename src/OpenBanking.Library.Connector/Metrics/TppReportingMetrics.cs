// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Metrics;

namespace FinnovationLabs.OpenBanking.Library.Connector.Metrics;

public class TppReportingMetrics
{
    public TppReportingMetrics(IMeterFactory meterFactory)
    {
        Meter meter = meterFactory.Create("TppReportingMetrics");
        Request2xxResponseCount = meter.CreateCounter<int>(
            "http.client.request.status2xx_responses",
            null,
            "Number of external API requests returning 2xx HTTP status codes");
        Request4xxResponseCount = meter.CreateCounter<int>(
            "http.client.request.status4xx_responses",
            null,
            "Number of external API requests returning 4xx HTTP status codes");
        Request5xxResponseCount = meter.CreateCounter<int>(
            "http.client.request.status5xx_responses",
            null,
            "Number of external API requests returning 5xx HTTP status codes");
        RequestNoResponseCount = meter.CreateCounter<int>(
            "http.client.request.no_response_count",
            null,
            "Number of external API requests returning no response");
    }

    public Counter<int> Request2xxResponseCount { get; private set; }
    public Counter<int> Request4xxResponseCount { get; private set; }
    public Counter<int> Request5xxResponseCount { get; private set; }
    public Counter<int> RequestNoResponseCount { get; private set; }
}
