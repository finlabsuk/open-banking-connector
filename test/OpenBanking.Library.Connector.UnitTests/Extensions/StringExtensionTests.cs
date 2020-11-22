// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FluentAssertions;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Extensions
{
    public class StringExtensionTests
    {
        [Theory]
        [InlineData("one", "one")]
        [InlineData("One", "one")]
        [InlineData("TwoWords", "two-words")]
        [InlineData("twoWords", "two-words")]
        [InlineData("NowThreeWords", "now-three-words")]
        [InlineData("nowThreeWords", "now-three-words")]
        [InlineData("ABC", "a-b-c")]
        public async Task PascalOrCamelToKebabCase_String_ReturnsString(string input, string expectedOutput)
        {
            // Arrange

            // Act
            var output = input.PascalOrCamelToKebabCase();

            // Assert 
            output.Should().Be(expectedOutput);
        }
    }
}
