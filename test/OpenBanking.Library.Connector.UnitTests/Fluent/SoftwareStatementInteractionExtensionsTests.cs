// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Fluent;

public class SoftwareStatementInteractionExtensionsTests
{
    // [Fact]
    // public async Task FluentExploration_MakeSoftwareStatement()
    // {
    //     var requestBuilder = TestDataFactory.CreateMockRequestBuilder();
    //
    //     var response = await requestBuilder.SoftwareStatementProfile()
    //         .Id("0")
    //         .SoftwareStatement("e30=.e30=.e30=")
    //         .SigningKeyInfo("id", "<signing key>", "<cert>")
    //         .TransportKeyInfo("<transport key>", "<transport cert>")
    //         .DefaultFragmentRedirectUrl("https://test.com")
    //         .SubmitAsync();
    //
    //     response.Should().NotBeNull();
    //     response.Messages.Should().BeEmpty();
    // }
    //
    // [Fact]
    // public void SoftwareStatement_ValueRetained()
    // {
    //     var obc = TestDataFactory.CreateMockOpenBankingContext();
    //     var ctx = new SoftwareStatementProfileContext(obc);
    //
    //     var statement = "aaa";
    //     ctx.SoftwareStatement(statement);
    //
    //     ctx.Data.SoftwareStatement.Should().Be(statement);
    // }
}
