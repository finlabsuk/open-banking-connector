// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Security
{
    public class KeySecretTests
    {
        [Theory]
        [InlineData(null, null, "")]
        [InlineData(null, "", null)]
        [InlineData("", null, null)]
        [InlineData(null, null, null)]
        public void Ctor_NullParameters_ExceptionThrown(string vault, string key, string value)
        {
            Action a = () => new KeySecret(vaultName: vault, key: key, value: value);

            a.Should().Throw<ArgumentNullException>();
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property CtorValuesInjected(string vault, string key, string value)
        {
            Func<bool> rule = () =>
            {
                var ks = new KeySecret(vaultName: vault, key: key, value: value);

                return ks.VaultName == vault && ks.Key == key && ks.Value == value;
            };

            return rule.When(vault != null && key != null && value != null);
        }
    }
}
