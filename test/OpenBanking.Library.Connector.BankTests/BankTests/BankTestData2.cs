// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

/// <summary>
///     Data for bank test apart from bankProfileEnum (passed separately to allow for
///     separate display in test runner).
/// </summary>
public class BankTestData2
{
    public required BankProfileEnum BankProfileEnum { get; init; }

    public required string BankRegistrationExternalApiId { get; init; }

    public string? BankRegistrationExternalApiSecretName { get; set; }

    public string? BankRegistrationRegistrationAccessTokenName { get; set; }

    public string? AccountAccessConsentExternalApiId { get; set; }

    public string? AccountAccessConsentAuthContextNonce { get; set; }

    public required RegistrationScopeEnum RegistrationScope { get; set; }

    public string? AuthUiInputUserName { get; set; }

    public string? AuthUiInputPassword { get; set; }

    public string? AuthUiExtraWord1 { get; set; }

    public string? AuthUiExtraWord2 { get; set; }

    public string? AuthUiExtraWord3 { get; set; }

    public required TestType TestType { get; init; }

    public required bool TestAuth { get; init; }

    public string? TestCreditorAccount { get; init; }

    public override string ToString()
    {
        string? extraBankProfileInfo = null;
        if (TestType is TestType.AccountAccessConsent or TestType.DomesticPaymentConsent or TestType.DomesticVrpConsent)
        {
            if (!TestAuth)
            {
                var elements = new List<string>();
                elements.Add("no auth");
                extraBankProfileInfo = "(" + string.Join(", ", elements) + ")";
            }
        }

        string label = $" {BankProfileEnum}" + (extraBankProfileInfo is null
            ? string.Empty
            : $" {extraBankProfileInfo}");
        return label;
    }
}
