﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models;
using FluentAssertions;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests
{
    public class ExtensionsTests
    {
        [Theory]
        [InlineData("", 0)]
        [InlineData("  ", 0)]
        [InlineData(".", 1)]
        [InlineData("...", 3)]
        [InlineData(" . . . ", 3)]
        [InlineData(" abc ", 0)]
        [InlineData(" ab. ", 1)]
        [InlineData(" .ab ", 1)]
        public void DelimiterCount(string value, int expectedCount)
        {
            int result = value.DelimiterCount('.');

            result.Should().Be(expectedCount);
        }

        [Fact]
        public void ToObjectDictionary_ScratchTest()
        {
            var value = new OBClientRegistration1
            {
                Aud = "some aud",
                ApplicationType = ApplicationTypeEnum.Web,
                ClientId = "some clientId"
            };

            var result = value.ToObjectDictionary();

            result.Should().NotBeNull();
            result["aud"].Should().Be(value.Aud);
            result["application_type"].Should().Be(value.ApplicationType);
            result["client_id"].Should().Be(value.ClientId);
            result["token_endpoint_auth_signing_alg"].Should().BeNull();
        }
    }
}
