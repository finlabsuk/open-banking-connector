// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.Http;

// Minimal IMeterFactory so tests can construct TppReportingMetrics without pulling in a DI
// container just to satisfy the constructor.
internal sealed class TestMeterFactory : IMeterFactory
{
    public Meter Create(MeterOptions options) => new(options);

    public void Dispose() { }
}

// Captures measurements recorded on a TppReportingMetrics instance via a MeterListener (Counter<T>
// is write-only to consumers). Filters by a per-test-unique "external_api_endpoint" tag rather
// than Meter identity, so it stays correct even if other tests create their own TppReportingMetrics
// concurrently (every instance shares the Meter name "TppReportingMetrics").
internal sealed class MetricsCapture : IDisposable
{
    private readonly string _endpointDescription;
    private readonly MeterListener _listener;
    private readonly List<(string InstrumentName, int Value)> _measurements = [];

    public MetricsCapture(string endpointDescription)
    {
        _endpointDescription = endpointDescription;
        _listener = new MeterListener
        {
            InstrumentPublished = (instrument, listener) =>
            {
                if (instrument.Meter.Name == "TppReportingMetrics")
                {
                    listener.EnableMeasurementEvents(instrument);
                }
            }
        };
        _listener.SetMeasurementEventCallback<int>(OnMeasurementRecorded);
        _listener.Start();
    }

    public void Dispose() => _listener.Dispose();

    public int TotalFor(string instrumentName) =>
        _measurements.Where(m => m.InstrumentName == instrumentName).Sum(m => m.Value);

    private void OnMeasurementRecorded(
        Instrument instrument,
        int measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags,
        object? state)
    {
        foreach (KeyValuePair<string, object?> tag in tags)
        {
            if (tag.Key == "external_api_endpoint" &&
                Equals(tag.Value, _endpointDescription))
            {
                _measurements.Add((instrument.Name, measurement));
                return;
            }
        }
    }
}

// Shared by ApiClientNonMtlsTests and ApiClientMtlsTests. CreateTppReportingMetrics pairs with
// MetricsCapture above - the "real bank connection" constructor is the only one that accepts a
// TppReportingMetrics, and it always builds its own real handler, so this can't be exercised with
// a mocked HttpMessageHandler at all.
internal static class ApiClientTestHelpers
{
    public static TppReportingMetrics CreateTppReportingMetrics() => new(new TestMeterFactory());
}
