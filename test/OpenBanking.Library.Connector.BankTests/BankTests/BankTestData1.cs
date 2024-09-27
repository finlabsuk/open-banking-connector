// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

/// <summary>
///     Data for bank test apart from bankProfileEnum (passed separately to allow for
///     separate display in test runner).
/// </summary>
public class BankTestData1
{
    public required string SoftwareStatementProfileId { get; set; }

    public override string ToString()
    {
        string label =
            char.ToUpper(SoftwareStatementProfileId[0]) + SoftwareStatementProfileId[1..];
        const int labelLength = 11;
        return label
            .PadRight(labelLength)
            .Substring(0, labelLength);
    }
}
