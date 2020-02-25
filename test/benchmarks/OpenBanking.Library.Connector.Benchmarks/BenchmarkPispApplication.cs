// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Running;
using McMaster.Extensions.CommandLineUtils;

namespace FinnovationLabs.OpenBanking.Library.Connector.Benchmarks
{
    [Command(Name = "pisp", Description = "Run pisp benchmarks")]
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
    public class BenchmarkPispApplication
    {
        [Option("--partner", Description = "The PISP partner code")]
        public string Partner { get; set; }

        [Benchmark]
        public void RunPispPayment()
        {
        }


        public void OnExecute()
        {
            BenchmarkRunner.Run<BenchmarkPispApplication>();
        }
    }
}
