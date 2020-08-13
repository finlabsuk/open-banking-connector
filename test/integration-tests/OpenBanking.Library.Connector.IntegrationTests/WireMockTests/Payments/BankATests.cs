using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.WireMockTests.Payments
{
    public class BankATests
    {
        [Fact]
        public void RunTest()
        {
            var mockTest = new MockPaymentsTests(
                new MockPaymentsData(), 
                new MockPaymentsServer(new MockPaymentsData()));

            mockTest.RunMockPaymentTest();
        }
    }
}
