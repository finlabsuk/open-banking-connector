// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using WireMock.Logging;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.WireMockTests
{
    public interface IMockPaymentServer
    {
        void SetUpOpenIdMock();
        void SetupPaymentEndpointMock();
        void SetUpOBDomesticResponseEndpoint();
        void SetupRegistrationMock();
        void SetupTokenEndpointMock();
        void SetUpAuthEndpoint();
        IEnumerable<ILogEntry> GetLogEntries();
    }
}
