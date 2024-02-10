// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.VariableRecurringPayments;

public enum DomesticVrpTemplateType
{
    SweepingVrp
}

public class DomesticVrpTemplateParameters
{
    public required string InstructionIdentification { get; init; }

    public required string EndToEndIdentification { get; init; }

    public required bool IncludeCreditorInInitiation { get; init; }

    public required bool IncludeDebtorInInitiation { get; init; }

    public required DateTimeOffset ValidFromDateTime { get; init; }

    public required DateTimeOffset ValidToDateTime { get; init; }
}

public class DomesticVrpTemplateRequest
{
    /// <summary>
    ///     Template type to use.
    /// </summary>
    public required DomesticVrpTemplateType Type { get; init; }

    public required DomesticVrpTemplateParameters Parameters { get; init; }
}

public static partial class DomesticVrpTemplates
{
    public static VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest
        DomesticVrpConsentExternalApiRequest(DomesticVrpTemplateRequest domesticVrpConsentTemplateRequest) =>
        domesticVrpConsentTemplateRequest.Type switch
        {
            DomesticVrpTemplateType.SweepingVrp => new
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest
                {
                    Data = new VariableRecurringPaymentsModelsPublic.Data2
                    {
                        ReadRefundAccount =
                            VariableRecurringPaymentsModelsPublic.Data2ReadRefundAccount
                                .Yes,
                        ControlParameters =
                            new VariableRecurringPaymentsModelsPublic.OBDomesticVRPControlParameters
                            {
                                PSUAuthenticationMethods = new List<string> { "UK.OBIE.SCA" },
                                PSUInteractionTypes =
                                    new List<VariableRecurringPaymentsModelsPublic.OBVRPInteractionTypes>
                                    {
                                        VariableRecurringPaymentsModelsPublic.OBVRPInteractionTypes.OffSession
                                    },
                                VRPType = new List<string> { "UK.OBIE.VRPType.Sweeping" },
                                ValidFromDateTime = domesticVrpConsentTemplateRequest.Parameters.ValidFromDateTime,
                                ValidToDateTime = domesticVrpConsentTemplateRequest.Parameters.ValidToDateTime,
                                MaximumIndividualAmount =
                                    new VariableRecurringPaymentsModelsPublic.OBActiveOrHistoricCurrencyAndAmount
                                    {
                                        Amount = "100.00",
                                        Currency = "GBP"
                                    },
                                PeriodicLimits =
                                    new List<VariableRecurringPaymentsModelsPublic.PeriodicLimits>
                                    {
                                        new()
                                        {
                                            Amount = "200.00",
                                            Currency = "GBP",
                                            PeriodAlignment = VariableRecurringPaymentsModelsPublic
                                                .PeriodicLimitsPeriodAlignment
                                                .Consent,
                                            PeriodType = VariableRecurringPaymentsModelsPublic.PeriodicLimitsPeriodType
                                                .Week
                                        }
                                    }
                            },
                        Initiation = new VariableRecurringPaymentsModelsPublic.OBDomesticVRPInitiation
                        {
                            DebtorAccount =
                                domesticVrpConsentTemplateRequest.Parameters.IncludeDebtorInInitiation
                                    ? new VariableRecurringPaymentsModelsPublic.OBCashAccountDebtorWithName
                                    {
                                        SchemeName = "UK.OBIE.IBAN",
                                        Identification = "GB76LOYD30949301273801",
                                        Name = "Marcus Sweepimus"
                                    }
                                    : null,
                            CreditorAccount = domesticVrpConsentTemplateRequest.Parameters.IncludeCreditorInInitiation
                                ? new VariableRecurringPaymentsModelsPublic.OBCashAccountCreditor3
                                {
                                    SchemeName = "SortCodeAccountNumber",
                                    Identification = "30949330000010",
                                    SecondaryIdentification = "Roll 90210",
                                    Name = "Marcus Sweepimus"
                                }
                                : null,
                            RemittanceInformation =
                                new VariableRecurringPaymentsModelsPublic.RemittanceInformation
                                {
                                    Reference = "Sweepco"
                                }
                        }
                    },
                    Risk = new VariableRecurringPaymentsModelsPublic.OBRisk1
                    {
                        PaymentContextCode = VariableRecurringPaymentsModelsPublic.OBRisk1PaymentContextCode
                            .TransferToThirdParty
                    }
                },
            _ => throw new ArgumentOutOfRangeException(
                nameof(domesticVrpConsentTemplateRequest.Type),
                domesticVrpConsentTemplateRequest.Type,
                null)
        };
}
