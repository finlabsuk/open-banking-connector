// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

/// <summary>
///     Data for bank test apart from bankProfileEnum (passed separately to allow for
///     separate display in test runner).
/// </summary>
public class BankTestData : IXunitSerializable
{
    public string SoftwareStatementProfileId { get; set; } = null!;

    public string? SoftwareStatementAndCertificateProfileOverride { get; set; }

    public RegistrationScopeEnum RegistrationScope { get; set; }

    public BankRegistration? BankRegistrationObject { get; set; }

    public Guid? BankRegistrationId { get; set; }

    public Guid? AccountAccessConsentId { get; set; }

    public void Deserialize(IXunitSerializationInfo info)
    {
        SoftwareStatementProfileId = info.GetValue<string>(nameof(SoftwareStatementProfileId));
        SoftwareStatementAndCertificateProfileOverride =
            info.GetValue<string>(nameof(SoftwareStatementAndCertificateProfileOverride));
        RegistrationScope = info.GetValue<RegistrationScopeEnum>(nameof(RegistrationScope));
        var bankRegistrationObjectString = info.GetValue<string?>(nameof(BankRegistrationObject));
        BankRegistrationObject =
            bankRegistrationObjectString is null
                ? null
                : JsonConvert.DeserializeObject<BankRegistration>(bankRegistrationObjectString);
        var bankRegistrationIdString = info.GetValue<string?>(nameof(BankRegistrationId));
        BankRegistrationId = bankRegistrationIdString is null ? null : Guid.Parse(bankRegistrationIdString);
        var accountAccessConsentIdString = info.GetValue<string?>(nameof(AccountAccessConsentId));
        AccountAccessConsentId =
            accountAccessConsentIdString is null ? null : Guid.Parse(accountAccessConsentIdString);
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(SoftwareStatementProfileId), SoftwareStatementProfileId);
        info.AddValue(
            nameof(SoftwareStatementAndCertificateProfileOverride),
            SoftwareStatementAndCertificateProfileOverride);
        info.AddValue(nameof(RegistrationScope), RegistrationScope);
        info.AddValue(
            nameof(BankRegistrationObject),
            BankRegistrationObject is null ? null : JsonConvert.SerializeObject(BankRegistrationObject));
        info.AddValue(nameof(BankRegistrationId), BankRegistrationId?.ToString());
        info.AddValue(nameof(AccountAccessConsentId), AccountAccessConsentId?.ToString());
    }

    public override string ToString()
    {
        string label = $"{SoftwareStatementProfileId}" + (SoftwareStatementAndCertificateProfileOverride is null
            ? string.Empty
            : $" : {SoftwareStatementAndCertificateProfileOverride}");
        const int labelLength = 20;
        return label
            .PadRight(labelLength)
            .Substring(0, labelLength);
    }
}
