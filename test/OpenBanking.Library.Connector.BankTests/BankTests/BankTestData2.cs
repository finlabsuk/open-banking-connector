// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

/// <summary>
///     Data for bank test apart from bankProfileEnum (passed separately to allow for
///     separate display in test runner).
/// </summary>
public class BankTestData2 : IXunitSerializable
{
    public BankProfileEnum BankProfileEnum { get; set; }

    public BankRegistration? BankRegistrationObject { get; set; }

    public Guid? BankRegistrationId { get; set; }

    public AccountAccessConsent? AccountAccessConsentObject { get; set; }
    public Guid? AccountAccessConsentId { get; set; }

    public string? BankRegistrationExternalApiId { get; set; }

    public string? BankRegistrationExternalApiSecret { get; set; }
    public string? AccountAccessConsentExternalApiId { get; set; }

    public string? AccountAccessConsentRefreshToken { get; set; }

    public void Deserialize(IXunitSerializationInfo info)
    {
        BankProfileEnum = info.GetValue<BankProfileEnum>(nameof(BankProfileEnum));
        var bankRegistrationObjectString = info.GetValue<string?>(nameof(BankRegistrationObject));
        BankRegistrationObject =
            bankRegistrationObjectString is null
                ? null
                : JsonConvert.DeserializeObject<BankRegistration>(bankRegistrationObjectString);
        var bankRegistrationIdString = info.GetValue<string?>(nameof(BankRegistrationId));
        BankRegistrationId = bankRegistrationIdString is null ? null : Guid.Parse(bankRegistrationIdString);

        var accountAccessConsentString = info.GetValue<string?>(nameof(AccountAccessConsentObject));
        AccountAccessConsentObject =
            accountAccessConsentString is null
                ? null
                : JsonConvert.DeserializeObject<AccountAccessConsent>(accountAccessConsentString);


        var accountAccessConsentIdString = info.GetValue<string?>(nameof(AccountAccessConsentId));
        AccountAccessConsentId =
            accountAccessConsentIdString is null ? null : Guid.Parse(accountAccessConsentIdString);

        BankRegistrationExternalApiId = info.GetValue<string?>(nameof(BankRegistrationExternalApiId));
        BankRegistrationExternalApiSecret = info.GetValue<string?>(nameof(BankRegistrationExternalApiSecret));
        AccountAccessConsentExternalApiId = info.GetValue<string?>(nameof(AccountAccessConsentExternalApiId));
        AccountAccessConsentRefreshToken =
            info.GetValue<string?>(nameof(AccountAccessConsentRefreshToken));
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(BankProfileEnum), BankProfileEnum);
        info.AddValue(
            nameof(BankRegistrationObject),
            BankRegistrationObject is null ? null : JsonConvert.SerializeObject(BankRegistrationObject));
        info.AddValue(nameof(BankRegistrationId), BankRegistrationId?.ToString());
        info.AddValue(
            nameof(AccountAccessConsentObject),
            AccountAccessConsentObject is null ? null : JsonConvert.SerializeObject(AccountAccessConsentObject));
        info.AddValue(nameof(AccountAccessConsentId), AccountAccessConsentId?.ToString());

        info.AddValue(nameof(BankRegistrationExternalApiId), BankRegistrationExternalApiId);
        info.AddValue(nameof(BankRegistrationExternalApiSecret), BankRegistrationExternalApiSecret);
        info.AddValue(nameof(AccountAccessConsentExternalApiId), AccountAccessConsentExternalApiId);
        info.AddValue(nameof(AccountAccessConsentRefreshToken), AccountAccessConsentRefreshToken);
    }

    public override string ToString()
    {
        string? extraBankProfileInfo = null;
        if (BankRegistrationObject is not null ||
            BankRegistrationId is not null ||
            AccountAccessConsentObject is not null ||
            AccountAccessConsentId is not null ||
            BankRegistrationExternalApiId is not null ||
            AccountAccessConsentExternalApiId is not null)
        {
            var elements = new List<string>();
            if (BankRegistrationObject is not null)
            {
                elements.Add("BankRegistration: object");
            }

            if (BankRegistrationId is not null)
            {
                elements.Add($"BankRegistrationId: {BankRegistrationId}");
            }

            if (AccountAccessConsentObject is not null)
            {
                elements.Add("AccountAccessConsent: object");
            }

            if (AccountAccessConsentId is not null)
            {
                elements.Add($"AccountAccessConsentId: {AccountAccessConsentId}");
            }

            if (BankRegistrationExternalApiId is not null)
            {
                elements.Add($"BankRegApiId: {BankRegistrationExternalApiId}");
            }

            if (AccountAccessConsentExternalApiId is not null)
            {
                elements.Add($"AispConsentApiId: {AccountAccessConsentExternalApiId}");
            }

            extraBankProfileInfo = "(" + string.Join(", ", elements) + ")";
        }

        string label = $"{BankProfileEnum}" + (extraBankProfileInfo is null
            ? string.Empty
            : $" {extraBankProfileInfo}");
        return label;
    }
}
