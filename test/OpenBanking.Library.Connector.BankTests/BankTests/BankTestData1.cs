// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

/// <summary>
///     Data for bank test apart from bankProfileEnum (passed separately to allow for
///     separate display in test runner).
/// </summary>
public class BankTestData1 : IXunitSerializable
{
    public string TestGroupName { get; set; } = null!;
    public string SoftwareStatementProfileId { get; set; } = null!;

    public string? SoftwareStatementAndCertificateProfileOverride { get; set; }

    public RegistrationScopeEnum RegistrationScope { get; set; }

    public void Deserialize(IXunitSerializationInfo info)
    {
        TestGroupName = info.GetValue<string>(nameof(TestGroupName));
        SoftwareStatementProfileId = info.GetValue<string>(nameof(SoftwareStatementProfileId));
        SoftwareStatementAndCertificateProfileOverride =
            info.GetValue<string>(nameof(SoftwareStatementAndCertificateProfileOverride));
        RegistrationScope = info.GetValue<RegistrationScopeEnum>(nameof(RegistrationScope));
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(TestGroupName), TestGroupName);
        info.AddValue(nameof(SoftwareStatementProfileId), SoftwareStatementProfileId);
        info.AddValue(
            nameof(SoftwareStatementAndCertificateProfileOverride),
            SoftwareStatementAndCertificateProfileOverride);
        info.AddValue(nameof(RegistrationScope), RegistrationScope);
    }

    public override string ToString()
    {
        string label = $"{TestGroupName}" + (SoftwareStatementAndCertificateProfileOverride is null
            ? string.Empty
            : $" ({SoftwareStatementAndCertificateProfileOverride})");
        const int labelLength = 20;
        return label
            .PadRight(labelLength)
            .Substring(0, labelLength);
    }
}
