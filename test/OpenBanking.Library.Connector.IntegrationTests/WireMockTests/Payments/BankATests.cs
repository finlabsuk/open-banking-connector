// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.WireMockTests.Payments
{
    public class BankATests
    {
        [Fact]
        public void RunTest()
        {
            var mapper = new ApiVariantMapper();
            var data = new MockPaymentsData(mapper);
            var mockTest = new MockPaymentsTests(
                data,
                new MockPaymentsServer(data));

            mockTest.RunMockPaymentTest();
        }
    }
}
