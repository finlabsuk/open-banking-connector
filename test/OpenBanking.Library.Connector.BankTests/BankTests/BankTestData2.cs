// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

/// <summary>
///     Data for bank test apart from bankProfileEnum (passed separately to allow for
///     separate display in test runner).
/// </summary>
public class BankTestData2 : IXunitSerializable
{
    public BankProfileEnum BankProfileEnum { get; set; }

    public string? BankRegistrationExternalApiId { get; set; }

    public string? BankRegistrationExternalApiSecret { get; set; }

    public string? BankRegistrationRegistrationAccessToken { get; set; }

    public string? AccountAccessConsentExternalApiId { get; set; }

    public string? AccountAccessConsentRefreshToken { get; set; }

    public string? AccountAccessConsentAuthContextNonce { get; set; }

    public void Deserialize(IXunitSerializationInfo info)
    {
        BankProfileEnum = info.GetValue<BankProfileEnum>(nameof(BankProfileEnum));
        BankRegistrationExternalApiId = info.GetValue<string?>(nameof(BankRegistrationExternalApiId));
        BankRegistrationExternalApiSecret = info.GetValue<string?>(nameof(BankRegistrationExternalApiSecret));
        BankRegistrationRegistrationAccessToken =
            info.GetValue<string?>(nameof(BankRegistrationRegistrationAccessToken));
        AccountAccessConsentExternalApiId = info.GetValue<string?>(nameof(AccountAccessConsentExternalApiId));
        AccountAccessConsentRefreshToken = info.GetValue<string?>(nameof(AccountAccessConsentRefreshToken));
        AccountAccessConsentAuthContextNonce = info.GetValue<string?>(nameof(AccountAccessConsentAuthContextNonce));
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(BankProfileEnum), BankProfileEnum);
        info.AddValue(nameof(BankRegistrationExternalApiId), BankRegistrationExternalApiId);
        info.AddValue(nameof(BankRegistrationExternalApiSecret), BankRegistrationExternalApiSecret);
        info.AddValue(nameof(BankRegistrationRegistrationAccessToken), BankRegistrationRegistrationAccessToken);
        info.AddValue(nameof(AccountAccessConsentExternalApiId), AccountAccessConsentExternalApiId);
        info.AddValue(nameof(AccountAccessConsentRefreshToken), AccountAccessConsentRefreshToken);
        info.AddValue(nameof(AccountAccessConsentAuthContextNonce), AccountAccessConsentAuthContextNonce);
    }

    public override string ToString()
    {
        string? extraBankProfileInfo = null;
        if (BankRegistrationExternalApiId is not null ||
            AccountAccessConsentExternalApiId is not null)
        {
            var elements = new List<string>();
            if (BankRegistrationExternalApiId is not null)
            {
                elements.Add($"RegExtId: {BankRegistrationExternalApiId}");
            }

            if (AccountAccessConsentExternalApiId is not null)
            {
                elements.Add($"AAConsentExtId: {AccountAccessConsentExternalApiId}");
            }

            extraBankProfileInfo = "(" + string.Join(", ", elements) + ")";
        }

        string label = $"{BankProfileEnum}" + (extraBankProfileInfo is null
            ? string.Empty
            : $" {extraBankProfileInfo}");
        return label;
    }
}
