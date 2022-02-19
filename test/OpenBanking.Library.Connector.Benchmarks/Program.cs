// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using BenchmarkDotNet.Running;

namespace FinnovationLabs.OpenBanking.Library.Connector.Benchmarks
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                // Should this be simplified to BenchmarkRunner.Run<TestClass>() ?
                BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return 1;
            }

            return 0;
        }
    }
}
