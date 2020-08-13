// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;
using FluentAssertions;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Model.Mapping
{
    public class EntityTypeFinderTests
    {
        [Fact]
        public void GetOpenBankingEquivalentTypes_GetsEquivalentTypes()
        {
            var finder = new EntityTypeFinder();

            var publicType = typeof(Meta);

            var typePairs = finder.GetOpenBankingEquivalentTypes(publicType).ToList();

            typePairs.Should().HaveCount(2);
        }
    }
}
