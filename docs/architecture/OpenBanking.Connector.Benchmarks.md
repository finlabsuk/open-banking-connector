# OpenBanking.Library.Connector.Benchmarks

Benchmarks is a standalone project intended to garner software performance benchmarks.

It’s intended to benchmark OpenBanking.Library.Connector code, and not the PISPs it integrates with. Its reports will help guide optimisation efforts and provide proof-of-quality to potential users.

## Implementation
* .Net Core 2.2  / C# 7 console application
* Incorporates BenchmarkDotNet (https://benchmarkdotnet.org/)

 
## Runtime profile
* Executed upon each build profiling with in-process mocks
* Executed upon integration test runs, for profiling against sandbox & live APIs

 
## Reports
For selected in-process code pathways:

* Mean/StdDev/Min/Max/Median execution times
* Memory & garbage collection pressure
* .Net Core 2.2 & (.Net Core 3 in future)

For selected sandbox APIs:

* Mean/StdDev/Min/Max/Median/Quartile execution times
* Memory & garbage collection pressure
* .Net Core 2.2 (.Net Core 3 in future)

