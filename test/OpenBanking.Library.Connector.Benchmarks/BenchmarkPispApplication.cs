// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Running;

namespace FinnovationLabs.OpenBanking.Library.Connector.Benchmarks;

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
    [Benchmark]
    public void RunPispPayment() { }


    public void OnExecute()
    {
        BenchmarkRunner.Run<BenchmarkPispApplication>();
    }
}
