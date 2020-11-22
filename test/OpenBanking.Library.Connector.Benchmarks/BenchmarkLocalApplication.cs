// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Running;

namespace FinnovationLabs.OpenBanking.Library.Connector.Benchmarks
{
    [InProcess]
    [MemoryDiagnoser]
    [RankColumn]
    [MinColumn]
    [MaxColumn]
    [Q1Column]
    [Q3Column]
    [AllStatisticsColumn]
    [JsonExporterAttribute.Full]
    [CsvMeasurementsExporter]
    [CsvExporter(CsvSeparator.Comma)]
    [HtmlExporter]
    [MarkdownExporterAttribute.GitHub]
    [GcServer(true)]
    public class BenchmarkLocalApplication
    {
        [Params(1, 2)]
        public int Params { get; set; }

        [Benchmark]
        public void RunLocalPayment()
        {
            /* TODO: We'll execute a simple payment here, using the parameters above. We'll have set up in-memory mocks for HTTP.
            We're not interested in the results (other than exceptions) we're only interested in execution profiles.
            */
        }

        [Benchmark]
        public void RunLocalConsent()
        {
            /* TODO: We'll execute a simple consent here, using the parameters above. We'll have set up in-memory mocks for HTTP.
            We're not interested in the results (other than exceptions) we're only interested in execution profiles.
            */
        }

        private void OnExecute()
        {
            BenchmarkRunner.Run<BenchmarkLocalApplication>();
        }
    }
}
