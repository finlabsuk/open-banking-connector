﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
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

    public string? AccountAccessConsentAuthContextNonce { get; set; }

    public RegistrationScopeEnum RegistrationScope { get; set; }

    public string? AuthUiInputUserName { get; set; }

    public string? AuthUiInputPassword { get; set; }

    public bool? AuthDisable { get; set; }

    public void Deserialize(IXunitSerializationInfo info)
    {
        BankProfileEnum = info.GetValue<BankProfileEnum>(nameof(BankProfileEnum));
        BankRegistrationExternalApiId = info.GetValue<string?>(nameof(BankRegistrationExternalApiId));
        BankRegistrationExternalApiSecret = info.GetValue<string?>(nameof(BankRegistrationExternalApiSecret));
        BankRegistrationRegistrationAccessToken =
            info.GetValue<string?>(nameof(BankRegistrationRegistrationAccessToken));
        AccountAccessConsentExternalApiId = info.GetValue<string?>(nameof(AccountAccessConsentExternalApiId));
        AccountAccessConsentAuthContextNonce = info.GetValue<string?>(nameof(AccountAccessConsentAuthContextNonce));
        RegistrationScope = info.GetValue<RegistrationScopeEnum>(nameof(RegistrationScope));
        AuthUiInputUserName = info.GetValue<string?>(nameof(AuthUiInputUserName));
        AuthUiInputPassword = info.GetValue<string?>(nameof(AuthUiInputPassword));
        AuthDisable = info.GetValue<bool?>(nameof(AuthDisable));
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(BankProfileEnum), BankProfileEnum);
        info.AddValue(nameof(BankRegistrationExternalApiId), BankRegistrationExternalApiId);
        info.AddValue(nameof(BankRegistrationExternalApiSecret), BankRegistrationExternalApiSecret);
        info.AddValue(nameof(BankRegistrationRegistrationAccessToken), BankRegistrationRegistrationAccessToken);
        info.AddValue(nameof(AccountAccessConsentExternalApiId), AccountAccessConsentExternalApiId);
        info.AddValue(nameof(AccountAccessConsentAuthContextNonce), AccountAccessConsentAuthContextNonce);
        info.AddValue(nameof(RegistrationScope), RegistrationScope);
        info.AddValue(nameof(AuthUiInputUserName), AuthUiInputUserName);
        info.AddValue(nameof(AuthUiInputPassword), AuthUiInputPassword);
        info.AddValue(nameof(AuthDisable), AuthDisable);
    }

    public override string ToString()
    {
        string? extraBankProfileInfo = null;
        if (BankRegistrationExternalApiId is not null ||
            AccountAccessConsentExternalApiId is not null)
        {
            var elements = new List<string>();

            bool supportsAisp = RegistrationScope.HasFlag(RegistrationScopeEnum.AccountAndTransaction);
            if (supportsAisp)
            {
                if (AccountAccessConsentExternalApiId is null ||
                    AuthDisable is true)
                {
                    elements.Add("AISP (no auth)");
                }
                else
                {
                    elements.Add("AISP");
                }
            }

            bool supportsPisp = RegistrationScope.HasFlag(RegistrationScopeEnum.PaymentInitiation);
            if (supportsPisp)
            {
                elements.Add("PISP");
            }

            extraBankProfileInfo = "(" + string.Join(", ", elements) + ")";
        }

        string label = $"{BankProfileEnum}" + (extraBankProfileInfo is null
            ? string.Empty
            : $" {extraBankProfileInfo}");
        return label;
    }
}
