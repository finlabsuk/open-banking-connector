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
public class BankTestData
{
    public required string ReferenceName { get; init; }

    public required string SoftwareStatement { get; init; }

    public required RegistrationScopeEnum RegistrationScope { get; init; }

    public required BankProfileEnum BankProfile { get; init; }

    public required string BankRegistrationExternalApiId { get; init; }

    public required string? BankRegistrationExternalApiSecretName { get; init; }

    public required string? BankRegistrationRegistrationAccessTokenName { get; init; }

    public required string? AccountAccessConsentExternalApiId { get; init; }

    public required string? AccountAccessConsentAuthContextNonce { get; init; }

    public required TestType TestType { get; init; }

    public required bool TestAuth { get; init; }

    public required string? TestCreditorAccount { get; init; }

    public required string? AuthUiInputUserName { get; init; }

    public required string? AuthUiInputPassword { get; init; }

    public required string? AuthUiExtraWord1 { get; init; }

    public required string? AuthUiExtraWord2 { get; init; }

    public required string? AuthUiExtraWord3 { get; init; }

    public override string ToString()
    {
        // Create software statement label
        string label1 =
            char.ToUpper(SoftwareStatement[0]) + SoftwareStatement[1..];
        const int label1Length = 11;
        label1 = label1
            .PadRight(label1Length)
            .Substring(0, label1Length);

        // Create bank profile label
        var label2 = ", ";
        if (TestType is TestType.AccountAccessConsent or TestType.DomesticPaymentConsent or TestType.DomesticVrpConsent)
        {
            if (!TestAuth)
            {
                label2 += "[l] ";
            }
        }
        label2 += $"{BankProfile}";
        const int label2Length = 36;
        label2 = label2
            .PadRight(label2Length)
            .Substring(0, label2Length);

        // Create creditor account label
        var label3 = "";
        if (TestType is TestType.DomesticPaymentConsent or TestType.DomesticVrpConsent)
        {
            label3 += $", {TestCreditorAccount!}";
        }

        return label1 + label2 + label3;
    }
}
