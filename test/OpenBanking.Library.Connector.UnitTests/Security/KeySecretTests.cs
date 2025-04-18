﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;
using FluentAssertions;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Security;

public class KeySecretTests
{
    [Theory]
    [InlineData(null, null, "")]
    [InlineData(null, "", null)]
    [InlineData("", null, null)]
    [InlineData(null, null, null)]
    public void Ctor_NullParameters_ExceptionThrown(string? vault, string? key, string? value)
    {
        Func<KeySecret> a = () => new KeySecret(vault!, key!, value!);

        a.Should().Throw<ArgumentNullException>();
    }

    [Property(Verbose = PropertyTests.VerboseTests)]
    public Property CtorValuesInjected(string vault, string key, string value)
    {
        Func<bool> rule = () =>
        {
            var ks = new KeySecret(vault, key, value);

            return ks.VaultName == vault && ks.Key == key && ks.Value == value;
        };

        return rule.When(vault != null && key != null && value != null);
    }
}
