using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
