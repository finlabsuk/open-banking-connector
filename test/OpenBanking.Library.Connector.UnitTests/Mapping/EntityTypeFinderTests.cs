// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FluentAssertions;
using Xunit;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Mapping
{
    public class EntityTypeFinderTests
    {
        [Fact]
        public void GetOpenBankingEquivalentTypes_GetsEquivalentTypes()
        {
            ApiVariantMappingConfiguration finder = new ApiVariantMappingConfiguration();

            Type publicType = typeof(PaymentInitiationModelsPublic.Meta);

            List<TypeMapping> typePairs = finder.GetTypesWithSourceApiEquivalent(publicType).ToList();

            typePairs.Should().HaveCount(1);
        }
    }
}
