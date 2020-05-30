// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.KeySecrets
{
    public class KeySecretReadOnlyRepositoryTests
    {
        [Fact]
        public async Task GetAsync_Property_ReturnsProperty()
        {
            // Arrange
            var prop1Key = Helpers.KeyWithoutId<SimpleClass>(nameof(SimpleClass.Prop1));
            var prop1Val = "val1";
            var mockStub = Substitute.For<IKeySecretReadOnlyProvider>();
            mockStub.GetKeySecretAsync(prop1Key).Returns(new KeySecret(key: prop1Key, value: prop1Val));
            var repo = new KeySecretReadRepository<SimpleClass>(mockStub);

            // Act
            var result = await repo.GetAsync("Prop1");

            // Assert 
            result.Should().Be(prop1Val);
        }

        [Fact]
        public async Task GetAsync_ReturnsObject()
        {
            // Arrange
            var prop1Key = Helpers.KeyWithoutId<SimpleClass>(nameof(SimpleClass.Prop1));
            var prop1Val = "val1";
            var prop2Key = Helpers.KeyWithoutId<SimpleClass>(nameof(SimpleClass.Prop2));
            var prop2Val = "val2";
            var testClass = new SimpleClass(prop1: prop1Val, prop2: prop2Val);

            var mockStub = Substitute.For<IKeySecretReadOnlyProvider>();
            mockStub.GetKeySecretAsync(prop1Key).Returns(new KeySecret(key: prop1Key, value: prop1Val));
            mockStub.GetKeySecretAsync(prop2Key).Returns(new KeySecret(key: prop2Key, value: prop2Val));

            var repo = new KeySecretReadRepository<SimpleClass>(mockStub);

            // Act
            var result = await repo.GetAsync();

            // Assert 
            result.Should().BeEquivalentTo(testClass);
        }
    }

    public class SimpleClass : IKeySecretItem
    {
        public SimpleClass(string prop1, string prop2)
        {
            Prop1 = prop1 ?? throw new ArgumentNullException(nameof(prop1));
            Prop2 = prop2 ?? throw new ArgumentNullException(nameof(prop2));
        }

        public string Prop1 { get; }
        public string Prop2 { get; }
    }
}
