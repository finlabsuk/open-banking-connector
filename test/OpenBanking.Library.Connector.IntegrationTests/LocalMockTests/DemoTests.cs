// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.LocalMockTests
{
    public class DemoTests : BaseLocalMockTest
    {
        [BddfyFact]
        public void DemoBddTest()
        {
            // TODO: A demo test. Backing xStory tests would provide
            this.Given("A PSU consent")
                .When("A payment is made")
                .Then("The successful payment response is verified")
                .BDDfy();
        }
    }
}
