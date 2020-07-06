// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model;
using FluentAssertions;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Fluent
{
    public class ObDomesticConsentInteractionExtensionsTests
    {
        [Fact]
        public void Amount_Repeated_ValueRetained()
        {
            var ctx = new DomesticPaymentConsentContext(TestDataFactory.CreateMockOpenBankingContext())
            {
                Data = null
            };


            var currency = "GBP";
            ctx.Amount(currency, 100.00);
            var value = 1234.00;
            ctx.Amount(currency, value);

            ctx.Data.Data.Initiation.InstructedAmount.Currency.Should().Be(currency);
            ctx.Data.Data.Initiation.InstructedAmount.Amount.Should().Be(value.ToString());
        }

        [Fact]
        public void Amount_ValueRetained()
        {
            var ctx = new DomesticPaymentConsentContext(TestDataFactory.CreateMockOpenBankingContext())
            {
                Data = null
            };


            var currency = "GBP";
            var value = 1234.00;
            ctx.Amount(currency, value);

            ctx.Data.Data.Initiation.InstructedAmount.Currency.Should().Be(currency);
            ctx.Data.Data.Initiation.InstructedAmount.Amount.Should().Be(value.ToString());
        }

        [Fact]
        public void DeliveryAddress_ValueRetained()
        {
            var ctx = new DomesticPaymentConsentContext(TestDataFactory.CreateMockOpenBankingContext())
            {
                Data = null
            };

            var value = new OBRiskDeliveryAddress();
            ctx.DeliveryAddress(value);

            ctx.Data.Risk.DeliveryAddress.Should().Be(value);
        }

        [Fact]
        public void InstructionIdentification_ValueRetained()
        {
            var ctx = new DomesticPaymentConsentContext(TestDataFactory.CreateMockOpenBankingContext())
            {
                Data = null
            };

            var value = "Abc";

            ctx.InstructionIdentification(value);

            ctx.Data.Data.Initiation.InstructionIdentification.Should().Be(value);
        }
    }
}
