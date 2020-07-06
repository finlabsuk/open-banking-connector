// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using McMaster.Extensions.CommandLineUtils;

namespace FinnovationLabs.OpenBanking.Library.Connector.Benchmarks
{
    [Command(Description = "Benchmark OpenBankingConnector")]
    [Subcommand(typeof(BenchmarkLocalApplication))]
    [Subcommand(typeof(BenchmarkPispApplication))]
    [Subcommand(typeof(EntityMappingApplication))]
    [HelpOption("--help")]
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                return CommandLineApplication.Execute<Program>(args);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return 1;
            }
        }
    }
}
