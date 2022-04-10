// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Fluent
{
    public class OpenBankingDomesticPaymentContextExtensionsTests
    {
        [Fact]
        public void ConsentId_NullData_ValueSet()
        {
            SharedContext ctx =
                TestDataFactory.CreateMockOpenBankingContext();

            var x = new LocalEntityDelete<DomesticPaymentConsent>(
                ctx.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                ctx.DbService.GetDbSaveChangesMethodClass(),
                ctx.TimeProvider,
                ctx.SoftwareStatementProfileCachedRepo,
                ctx.Instrumentation);

            // var consentIdValue = "abc";
            //
            // ctx.ConsentId(consentIdValue);
            //
            // ctx.ConsentId.Should().Be(consentIdValue);
        }
    }
}
