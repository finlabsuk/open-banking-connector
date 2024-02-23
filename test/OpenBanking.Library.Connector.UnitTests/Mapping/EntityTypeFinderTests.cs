// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FluentAssertions;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Mapping;

public class EntityTypeFinderTests
{
    [Fact]
    public void GetOpenBankingEquivalentTypes_GetsEquivalentTypes()
    {
        var finder = new ApiVariantMappingConfiguration();

        Type publicType = typeof(PaymentInitiationModelsPublic.Data);

        List<TypeMapping> typePairs = finder.GetTypesWithSourceApiEquivalent(publicType).ToList();

        typePairs.Should().HaveCount(1);
    }
}
