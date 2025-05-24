// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FsCheck;
using FsCheck.Fluent;
using NSubstitute;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Artbitraries;

public static class BaseMockArbitrary<T>
    where T : class
{
    public static Arbitrary<T> GetArbitrary()
    {
        T[] elements = { Substitute.For<T>() };

        return Gen.Elements(elements).ToArbitrary();
    }
}
