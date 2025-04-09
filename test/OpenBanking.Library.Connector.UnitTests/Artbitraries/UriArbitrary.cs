﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FsCheck;
using FsCheck.Fluent;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Artbitraries;

public static class UriArbitrary
{
    public static Arbitrary<Uri> GetArbitrary()
    {
        string[] hosts = { "localhost", "127.0.0.1", "mytest.com" };
        Uri[] uris = hosts.Select(h => new Uri("http://" + h)).ToArray();

        return Gen.Elements(uris).ToArbitrary();
    }
}
