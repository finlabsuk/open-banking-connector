// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;

public class Account
{
    public Account(string schemeName, string identification, string name)
    {
        SchemeName = schemeName;
        Identification = identification;
        Name = name;
    }

    public string SchemeName { get; }

    public string Identification { get; }

    public string Name { get; }

    public string? SecondaryIdentification { get; set; }
}

public class DomesticVrpAccountIndexPair
{
    public DomesticVrpAccountIndexPair(int source, int dest)
    {
        Source = source;
        Dest = dest;
    }

    public int Source { get; }

    public int Dest { get; }
}

public class BankUser
{
    public required string UserNameOrNumber { get; init; }

    public required string Password { get; init; }

    public required string ExtraWord1 { get; init; }

    public required string ExtraWord2 { get; init; }

    public required string ExtraWord3 { get; init; }
}
