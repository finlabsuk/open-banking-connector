// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Instrumentation
{
    public interface IInstrumentationClient
    {
        void Info(string message);
        void Warning(string message);
        void Error(string message);

        void Trace(string message);
        void Exception(Exception exception);
        void Exception(Exception exception, string message);
    }
}
