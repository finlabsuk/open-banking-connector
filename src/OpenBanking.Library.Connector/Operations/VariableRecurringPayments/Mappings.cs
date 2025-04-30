// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// using VariableRecurringPaymentsModelsV3p1p11 =
//     FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.NSwagVrp.Models;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V4p0.NSwagVrp.Models;

public static class Mappings
{
    public static VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPConsentRequest MapFromOBDomesticVRPConsentRequest(
        OBDomesticVRPConsentRequest externalApiRequest) => new()
    {
        Data = new VariableRecurringPaymentsModelsV3p1p11.Data2
        {
            ControlParameters = new VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPControlParameters
            {
                ValidFromDateTime = externalApiRequest.Data.ControlParameters.ValidFromDateTime,
                ValidToDateTime = externalApiRequest.Data.ControlParameters.ValidToDateTime,
                MaximumIndividualAmount =
                    externalApiRequest.Data.ControlParameters.MaximumIndividualAmount.MapFromAmount(),
                PeriodicLimits = externalApiRequest.Data.ControlParameters.PeriodicLimits
                    .Select(a => a.MapFromPeriodicLimits())
                    .ToList(),
                VRPType = externalApiRequest.Data.ControlParameters.VRPType,
                PSUAuthenticationMethods = externalApiRequest.Data.ControlParameters.PSUAuthenticationMethods,
                PSUInteractionTypes = externalApiRequest.Data.ControlParameters.PSUInteractionTypes?
                    .Select(a => a.MapFromPSUInteractionTypes())
                    .ToList(),
                SupplementaryData = externalApiRequest.Data.ControlParameters.SupplementaryData,
                AdditionalProperties = externalApiRequest.Data.ControlParameters.AdditionalProperties
            },
            ReadRefundAccount = externalApiRequest.Data.ReadRefundAccount?.MapFromReadRefundAccount(),
            Initiation = new VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPInitiation
            {
                DebtorAccount =
                    externalApiRequest.Data.Initiation.DebtorAccount?.MapFromDebtorAccount2(),
                CreditorAccount = externalApiRequest.Data.Initiation.CreditorAccount?.MapFromCreditorAccount(),
                CreditorPostalAddress =
                    externalApiRequest.Data.Initiation.CreditorPostalAddress?.MapFromCreditorPostalAddress(),
                RemittanceInformation =
                    externalApiRequest.Data.Initiation.RemittanceInformation?.MapFromRemittanceInformation(),
                AdditionalProperties = externalApiRequest.Data.Initiation.AdditionalProperties
            },
            AdditionalProperties = externalApiRequest.Data.AdditionalProperties
        },
        Risk = externalApiRequest.Risk.MapFromRisk(),
        AdditionalProperties = externalApiRequest.AdditionalProperties
    };

    public static VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPRequest MapFromOBDomesticVRPRequest(
        OBDomesticVRPRequest externalApiRequest) => new()
    {
        Data = new VariableRecurringPaymentsModelsV3p1p11.Data3
        {
            ConsentId = externalApiRequest.Data.ConsentId,
            PSUAuthenticationMethod = externalApiRequest.Data.PSUAuthenticationMethod,
            PSUInteractionType = externalApiRequest.Data.PSUInteractionType?.MapFromPSUInteractionTypes(),
            VRPType = externalApiRequest.Data.VRPType,
            Initiation = new VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPInitiation
            {
                DebtorAccount =
                    externalApiRequest.Data.Initiation.DebtorAccount?.MapFromDebtorAccount2(),
                CreditorAccount =
                    externalApiRequest.Data.Initiation.CreditorAccount?.MapFromCreditorAccount(),
                CreditorPostalAddress =
                    externalApiRequest.Data.Initiation.CreditorPostalAddress?.MapFromCreditorPostalAddress(),
                RemittanceInformation =
                    externalApiRequest.Data.Initiation.RemittanceInformation?.MapFromRemittanceInformation(),
                AdditionalProperties = externalApiRequest.Data.Initiation.AdditionalProperties
            },
            Instruction = new VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPInstruction
            {
                InstructionIdentification = externalApiRequest.Data.Instruction.InstructionIdentification,
                EndToEndIdentification = externalApiRequest.Data.Instruction.EndToEndIdentification,
                LocalInstrument = externalApiRequest.Data.Instruction.LocalInstrument,
                InstructedAmount = externalApiRequest.Data.Instruction.InstructedAmount.MapFromAmount(),
                CreditorAccount = externalApiRequest.Data.Instruction.CreditorAccount.MapFromCreditorAccount(),
                CreditorPostalAddress =
                    externalApiRequest.Data.Instruction.CreditorPostalAddress?.MapFromCreditorPostalAddress(),
                RemittanceInformation =
                    externalApiRequest.Data.Instruction.RemittanceInformation?.MapFromRemittanceInformation2(),
                SupplementaryData = externalApiRequest.Data.Instruction.SupplementaryData,
                AdditionalProperties = externalApiRequest.Data.Instruction.AdditionalProperties
            },
            AdditionalProperties = externalApiRequest.Data.AdditionalProperties
        },
        Risk = externalApiRequest.Risk.MapFromRisk(),
        AdditionalProperties = externalApiRequest.AdditionalProperties
    };

    public static VariableRecurringPaymentsModelsV3p1p11.OBVRPFundsConfirmationRequest
        MapFromOBVRPFundsConfirmationRequest(OBVRPFundsConfirmationRequest externalApiRequest) =>
        new()
        {
            Data = new VariableRecurringPaymentsModelsV3p1p11.Data6
            {
                ConsentId = externalApiRequest.Data.ConsentId,
                Reference = externalApiRequest.Data.Reference,
                InstructedAmount = externalApiRequest.Data.InstructedAmount.MapFromAmount(),
                AdditionalProperties = externalApiRequest.Data.AdditionalProperties
            },
            AdditionalProperties = externalApiRequest.AdditionalProperties
        };

    public static OBDomesticVRPConsentResponse MapToOBDomesticVRPConsentResponse(
        VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPConsentResponse externalApiResponseV3) =>
        new()
        {
            Data = new Data
            {
                ConsentId = externalApiResponseV3.Data.ConsentId,
                CreationDateTime = externalApiResponseV3.Data.CreationDateTime,
                Status = externalApiResponseV3.Data.Status.MapToStatus(),
                StatusReason = null, // not in v3
                StatusUpdateDateTime = externalApiResponseV3.Data.StatusUpdateDateTime,
                ControlParameters = new OBDomesticVRPControlParameters
                {
                    ValidFromDateTime = externalApiResponseV3.Data.ControlParameters.ValidFromDateTime,
                    ValidToDateTime = externalApiResponseV3.Data.ControlParameters.ValidToDateTime,
                    MaximumIndividualAmount =
                        externalApiResponseV3.Data.ControlParameters.MaximumIndividualAmount.MapToAmount(),
                    PeriodicLimits = externalApiResponseV3.Data.ControlParameters.PeriodicLimits
                        .Select(a => a.MapToPeriodicLimits())
                        .ToList(),
                    VRPType = externalApiResponseV3.Data.ControlParameters.VRPType,
                    PSUAuthenticationMethods = externalApiResponseV3.Data.ControlParameters.PSUAuthenticationMethods,
                    PSUInteractionTypes = externalApiResponseV3.Data.ControlParameters.PSUInteractionTypes?
                        .Select(a => a.MapToPSUInteractionTypes())
                        .ToList(),
                    SupplementaryData = externalApiResponseV3.Data.ControlParameters.SupplementaryData,
                    AdditionalProperties = externalApiResponseV3.Data.ControlParameters.AdditionalProperties
                },
                ReadRefundAccount = externalApiResponseV3.Data.ReadRefundAccount?.MapToReadRefundAccount(),
                Initiation = new OBDomesticVRPInitiation
                {
                    DebtorAccount = externalApiResponseV3.Data.Initiation.DebtorAccount?.MapToDebtorAccount(),
                    CreditorAccount =
                        externalApiResponseV3.Data.Initiation.CreditorAccount?.MapToCreditorAccount(),
                    CreditorPostalAddress =
                        externalApiResponseV3.Data.Initiation.CreditorPostalAddress?.MapToCreditorPostalAddress(),
                    UltimateCreditor = null, // not in v3
                    UltimateDebtor = null, // not in v3
                    RegulatoryReporting = null, // not in v3
                    RemittanceInformation =
                        externalApiResponseV3.Data.Initiation.RemittanceInformation?.MapToRemittanceInformation(),
                    AdditionalProperties = externalApiResponseV3.Data.Initiation.AdditionalProperties
                },
                DebtorAccount = externalApiResponseV3.Data.DebtorAccount?.MapToDebtorAccount2(),
                AdditionalProperties = externalApiResponseV3.Data.AdditionalProperties
            },
            Risk = externalApiResponseV3.Risk.MapToRisk(),
            Links = externalApiResponseV3.Links.MapToLinks(),
            Meta = externalApiResponseV3.Meta.MapToMeta(),
            AdditionalProperties = externalApiResponseV3.AdditionalProperties
        };

    public static OBDomesticVRPResponse MapToOBDomesticVRPResponse(
        VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPResponse externalApiResponseV3) =>
        new()
        {
            Data = new Data4
            {
                DomesticVRPId = externalApiResponseV3.Data.DomesticVRPId,
                ConsentId = externalApiResponseV3.Data.ConsentId,
                CreationDateTime = externalApiResponseV3.Data.CreationDateTime,
                Status = externalApiResponseV3.Data.Status.MapToStatus2(),
                StatusUpdateDateTime = externalApiResponseV3.Data.StatusUpdateDateTime,
                StatusReason = null, // decision: placeholder
                V3StatusReason =
                    externalApiResponseV3.Data
                        .StatusReason, // decision: map StatusReason to V3StatusReason to avoid mixing types
                ExpectedExecutionDateTime = externalApiResponseV3.Data.ExpectedExecutionDateTime,
                ExpectedSettlementDateTime = externalApiResponseV3.Data.ExpectedSettlementDateTime,
                Refund = externalApiResponseV3.Data.Refund?.MapToDebtorAccount(),
                Charges = externalApiResponseV3.Data.Charges?
                    .Select(a => a.MapToCharges())
                    .ToList(),
                Initiation = new OBDomesticVRPInitiation
                {
                    DebtorAccount = externalApiResponseV3.Data.Initiation.DebtorAccount?.MapToDebtorAccount2(),
                    CreditorAccount = externalApiResponseV3.Data.Initiation.CreditorAccount?.MapToCreditorAccount(),
                    CreditorPostalAddress =
                        externalApiResponseV3.Data.Initiation.CreditorPostalAddress?.MapToCreditorPostalAddress(),
                    UltimateCreditor = null, // not in v3
                    UltimateDebtor = null, // not in v3
                    RegulatoryReporting = null, // not in v3
                    AdditionalProperties = externalApiResponseV3.Data.Initiation.AdditionalProperties,
                    RemittanceInformation =
                        externalApiResponseV3.Data.Initiation.RemittanceInformation?.MapToRemittanceInformation()
                },
                Instruction = new OBDomesticVRPInstruction
                {
                    InstructionIdentification = externalApiResponseV3.Data.Instruction.InstructionIdentification,
                    EndToEndIdentification = externalApiResponseV3.Data.Instruction.EndToEndIdentification,
                    LocalInstrument = externalApiResponseV3.Data.Instruction.LocalInstrument,
                    InstructedAmount = externalApiResponseV3.Data.Instruction.InstructedAmount.MapToAmount(),
                    CreditorAccount = externalApiResponseV3.Data.Instruction.CreditorAccount.MapToCreditorAccount(),
                    CreditorPostalAddress =
                        externalApiResponseV3.Data.Instruction.CreditorPostalAddress?.MapToCreditorPostalAddress(),
                    UltimateCreditor = null, // not in v3
                    RemittanceInformation =
                        externalApiResponseV3.Data.Instruction.RemittanceInformation?.MapToRemittanceInformation2(),
                    SupplementaryData =
                        externalApiResponseV3.Data.Instruction.SupplementaryData,
                    AdditionalProperties = externalApiResponseV3.Data.Instruction.AdditionalProperties
                },
                DebtorAccount = externalApiResponseV3.Data.DebtorAccount?.MapToDebtorAccount2(),
                AdditionalProperties = externalApiResponseV3.Data.AdditionalProperties
            },
            Risk = externalApiResponseV3.Risk.MapToRisk(),
            Links = externalApiResponseV3.Links.MapToLinks(),
            Meta = externalApiResponseV3.Meta.MapToMeta(),
            AdditionalProperties = externalApiResponseV3.AdditionalProperties
        };

    public static OBVRPFundsConfirmationResponse MapToOBVRPFundsConfirmationResponse(
        VariableRecurringPaymentsModelsV3p1p11.OBVRPFundsConfirmationResponse externalApiResponseV3) =>
        new()
        {
            Data = new Data7
            {
                FundsConfirmationId = externalApiResponseV3.Data.FundsConfirmationId,
                ConsentId = externalApiResponseV3.Data.ConsentId,
                CreationDateTime = externalApiResponseV3.Data.CreationDateTime,
                Reference = externalApiResponseV3.Data.Reference,
                FundsAvailableResult = externalApiResponseV3.Data.FundsAvailableResult.MapToFundsAvailableResult(),
                InstructedAmount = externalApiResponseV3.Data.InstructedAmount.MapToAmount(),
                AdditionalProperties = externalApiResponseV3.Data.AdditionalProperties
            },
            AdditionalProperties = externalApiResponseV3.AdditionalProperties
        };

    public static OBDomesticVRPDetails MapToOBDomesticVRPDetails(
        VariableRecurringPaymentsModelsV3p1p11.OBDomesticVRPDetails externalApiResponseV3) =>
        new()
        {
            Data = externalApiResponseV3.Data?.MapToData5() ?? new Data5
            {
                PaymentStatus = null
                //AdditionalProperties
            },
            AdditionalProperties = externalApiResponseV3.AdditionalProperties
        };

    private static Data5 MapToData5(
        this VariableRecurringPaymentsModelsV3p1p11.Data5 data) => new()
    {
        PaymentStatus = data.PaymentStatus?
            .Select(a => a.MapToPaymentStatus())
            .ToList(),
        AdditionalProperties = data.AdditionalProperties
    };

    private static VariableRecurringPaymentsModelsV3p1p11.PeriodicLimits MapFromPeriodicLimits(
        this PeriodicLimits data) => new()
    {
        PeriodType = data.PeriodType switch
        {
            PeriodicLimitsPeriodType.Day => VariableRecurringPaymentsModelsV3p1p11.PeriodicLimitsPeriodType.Day,
            PeriodicLimitsPeriodType.Week => VariableRecurringPaymentsModelsV3p1p11.PeriodicLimitsPeriodType.Week,
            PeriodicLimitsPeriodType.Fortnight => VariableRecurringPaymentsModelsV3p1p11.PeriodicLimitsPeriodType
                .Fortnight,
            PeriodicLimitsPeriodType.Month => VariableRecurringPaymentsModelsV3p1p11.PeriodicLimitsPeriodType.Month,
            PeriodicLimitsPeriodType.HalfYear => VariableRecurringPaymentsModelsV3p1p11.PeriodicLimitsPeriodType
                .HalfYear,
            PeriodicLimitsPeriodType.Year => VariableRecurringPaymentsModelsV3p1p11.PeriodicLimitsPeriodType.Year,
            _ => throw new ArgumentOutOfRangeException()
        },
        PeriodAlignment = data.PeriodAlignment switch
        {
            PeriodicLimitsPeriodAlignment.Consent => VariableRecurringPaymentsModelsV3p1p11
                .PeriodicLimitsPeriodAlignment.Consent,
            PeriodicLimitsPeriodAlignment.Calendar => VariableRecurringPaymentsModelsV3p1p11
                .PeriodicLimitsPeriodAlignment.Calendar,
            _ => throw new ArgumentOutOfRangeException()
        },
        Amount = data.Amount,
        Currency = data.Currency,
        AdditionalProperties = data.AdditionalProperties
    };

    private static PeriodicLimits MapToPeriodicLimits(
        this VariableRecurringPaymentsModelsV3p1p11.PeriodicLimits data) => new()
    {
        PeriodType = data.PeriodType switch
        {
            VariableRecurringPaymentsModelsV3p1p11.PeriodicLimitsPeriodType.Day => PeriodicLimitsPeriodType.Day,
            VariableRecurringPaymentsModelsV3p1p11.PeriodicLimitsPeriodType.Week => PeriodicLimitsPeriodType.Week,
            VariableRecurringPaymentsModelsV3p1p11.PeriodicLimitsPeriodType.Fortnight => PeriodicLimitsPeriodType
                .Fortnight,
            VariableRecurringPaymentsModelsV3p1p11.PeriodicLimitsPeriodType.Month => PeriodicLimitsPeriodType.Month,
            VariableRecurringPaymentsModelsV3p1p11.PeriodicLimitsPeriodType.HalfYear => PeriodicLimitsPeriodType
                .HalfYear,
            VariableRecurringPaymentsModelsV3p1p11.PeriodicLimitsPeriodType.Year => PeriodicLimitsPeriodType.Year,
            _ => throw new ArgumentOutOfRangeException()
        },
        PeriodAlignment = data.PeriodAlignment switch
        {
            VariableRecurringPaymentsModelsV3p1p11.PeriodicLimitsPeriodAlignment.Consent =>
                PeriodicLimitsPeriodAlignment.Consent,
            VariableRecurringPaymentsModelsV3p1p11.PeriodicLimitsPeriodAlignment.Calendar =>
                PeriodicLimitsPeriodAlignment.Calendar,
            _ => throw new ArgumentOutOfRangeException()
        },
        Amount = data.Amount,
        Currency = data.Currency,
        AdditionalProperties = data.AdditionalProperties
    };

    private static VariableRecurringPaymentsModelsV3p1p11.OBVRPInteractionTypes MapFromPSUInteractionTypes(
        this OBVRPInteractionTypes data) => data switch
    {
        OBVRPInteractionTypes.InSession => VariableRecurringPaymentsModelsV3p1p11.OBVRPInteractionTypes.InSession,
        OBVRPInteractionTypes.OffSession => VariableRecurringPaymentsModelsV3p1p11.OBVRPInteractionTypes.OffSession,
        _ => throw new ArgumentOutOfRangeException(nameof(data), data, null)
    };

    private static OBVRPInteractionTypes MapToPSUInteractionTypes(
        this VariableRecurringPaymentsModelsV3p1p11.OBVRPInteractionTypes data) => data switch
    {
        VariableRecurringPaymentsModelsV3p1p11.OBVRPInteractionTypes.InSession => OBVRPInteractionTypes.InSession,
        VariableRecurringPaymentsModelsV3p1p11.OBVRPInteractionTypes.OffSession => OBVRPInteractionTypes.OffSession,
        _ => throw new ArgumentOutOfRangeException(nameof(data), data, null)
    };

    private static OBPAFundsAvailableResult1 MapToFundsAvailableResult(
        this VariableRecurringPaymentsModelsV3p1p11.OBPAFundsAvailableResult1 data) => new()
    {
        FundsAvailableDateTime = data.FundsAvailableDateTime,
        FundsAvailable = data.FundsAvailable switch
        {
            VariableRecurringPaymentsModelsV3p1p11.OBPAFundsAvailableResult1FundsAvailable.Available =>
                OBPAFundsAvailableResult1FundsAvailable.Available,
            VariableRecurringPaymentsModelsV3p1p11.OBPAFundsAvailableResult1FundsAvailable.NotAvailable =>
                OBPAFundsAvailableResult1FundsAvailable.NotAvailable,
            _ => throw new ArgumentOutOfRangeException()
        },
        AdditionalProperties = data.AdditionalProperties
    };

    private static PaymentStatus MapToPaymentStatus(
        this VariableRecurringPaymentsModelsV3p1p11.PaymentStatus data) => new()
    {
        PaymentTransactionId = data.PaymentTransactionId,
        Status = PaymentStatusStatusV4.PDNG, // decision: use PDNG (pending) as placeholder for required field
        V3Status =
            data.Status, // decision: map Status to V3Status to avoid information loss converting between two different enums
        StatusUpdateDateTime = data.StatusUpdateDateTime,
        StatusDetail = data.StatusDetail?.MapToStatusDetail()
        //AdditionalProperties
    };

    private static StatusDetail MapToStatusDetail(
        this VariableRecurringPaymentsModelsV3p1p11.StatusDetail data) => new()
    {
        LocalInstrument = data.LocalInstrument,
        Status = "PDNG", // decision: use PDNG (pending) as placeholder for required field
        V3Status = data.Status, // decision: map Status to V3Status to avoid information loss converting string to enum
        StatusReason = null, // decision: placeholder
        V3StatusReason =
            data.StatusReason, // decision: map StatusReason to V3StatusReason to avoid information loss converting between two different enums
        StatusReasonDescription = data.StatusReasonDescription
        //AdditionalProperties
    };

    private static DataStatus MapToStatus(this VariableRecurringPaymentsModelsV3p1p11.DataStatus data) =>
        data switch
        {
            VariableRecurringPaymentsModelsV3p1p11.DataStatus.Authorised => DataStatus.AUTH,
            VariableRecurringPaymentsModelsV3p1p11.DataStatus.AwaitingAuthorisation => DataStatus.AWAU,
            VariableRecurringPaymentsModelsV3p1p11.DataStatus.Rejected => DataStatus.RJCT,
            _ => throw new ArgumentOutOfRangeException()
        };

    private static Data4Status MapToStatus2(this VariableRecurringPaymentsModelsV3p1p11.Data4Status data) =>
        data switch
        {
            VariableRecurringPaymentsModelsV3p1p11.Data4Status.AcceptedCreditSettlementCompleted => Data4Status.ACCC,
            VariableRecurringPaymentsModelsV3p1p11.Data4Status.AcceptedSettlementCompleted => Data4Status.ACSC,
            VariableRecurringPaymentsModelsV3p1p11.Data4Status.AcceptedSettlementInProcess => Data4Status.ACSP,
            VariableRecurringPaymentsModelsV3p1p11.Data4Status.AcceptedWithoutPosting => Data4Status.ACWP,
            VariableRecurringPaymentsModelsV3p1p11.Data4Status.Pending => Data4Status.PDNG,
            VariableRecurringPaymentsModelsV3p1p11.Data4Status.Rejected => Data4Status.RJCT,
            _ => throw new ArgumentOutOfRangeException(nameof(data), data, null)
        };

    private static Charges MapToCharges(
        this VariableRecurringPaymentsModelsV3p1p11.Charges data) => new()
    {
        ChargeBearer = data.ChargeBearer.MapToChargeBearer(),
        Type = data.Type switch
        {
            VariableRecurringPaymentsModelsV3p1p11.OBExternalPaymentChargeType1Code.UK_OBIE_CHAPSOut =>
                OBInternalPaymentChargeType1Code.UK_OBIE_CHAPSOut,
            VariableRecurringPaymentsModelsV3p1p11.OBExternalPaymentChargeType1Code.UK_OBIE_BalanceTransferOut =>
                OBInternalPaymentChargeType1Code.UK_OBIE_BalanceTransferOut,
            VariableRecurringPaymentsModelsV3p1p11.OBExternalPaymentChargeType1Code.UK_OBIE_MoneyTransferOut =>
                OBInternalPaymentChargeType1Code.UK_OBIE_MoneyTransferOut,
            _ => throw new ArgumentOutOfRangeException()
        },
        Amount = data.Amount.MapToAmount(),
        AdditionalProperties = data.AdditionalProperties
    };

    private static OBInternalChargeBearerType1Code MapToChargeBearer(
        this VariableRecurringPaymentsModelsV3p1p11.OBChargeBearerType1Code data) =>
        data switch
        {
            VariableRecurringPaymentsModelsV3p1p11.OBChargeBearerType1Code.BorneByCreditor =>
                OBInternalChargeBearerType1Code
                    .BorneByCreditor,
            VariableRecurringPaymentsModelsV3p1p11.OBChargeBearerType1Code.BorneByDebtor =>
                OBInternalChargeBearerType1Code
                    .BorneByDebtor,
            VariableRecurringPaymentsModelsV3p1p11.OBChargeBearerType1Code.FollowingServiceLevel =>
                OBInternalChargeBearerType1Code.FollowingServiceLevel,
            VariableRecurringPaymentsModelsV3p1p11.OBChargeBearerType1Code.Shared => OBInternalChargeBearerType1Code
                .Shared,
            _ => throw new ArgumentOutOfRangeException()
        };

    private static OBActiveOrHistoricCurrencyAndAmount MapToAmount(
        this VariableRecurringPaymentsModelsV3p1p11.OBActiveOrHistoricCurrencyAndAmount data) => new()
    {
        Amount = data.Amount,
        Currency = data.Currency,
        AdditionalProperties = data.AdditionalProperties
    };

    private static VariableRecurringPaymentsModelsV3p1p11.OBActiveOrHistoricCurrencyAndAmount MapFromAmount(
        this OBActiveOrHistoricCurrencyAndAmount data) => new()
    {
        Amount = data.Amount,
        Currency = data.Currency,
        AdditionalProperties = data.AdditionalProperties
    };

    private static OBRisk1 MapToRisk(
        this VariableRecurringPaymentsModelsV3p1p11.OBRisk1 data) => new()
    {
        PaymentContextCode = data.PaymentContextCode?.MapToPaymentContextCode(),
        MerchantCategoryCode = data.MerchantCategoryCode,
        MerchantCustomerIdentification = data.MerchantCustomerIdentification,
        ContractPresentIndicator = data.ContractPresentIndicator,
        ContractPresentInidicator = data.ContractPresentInidicator,
        BeneficiaryPrepopulatedIndicator = data.BeneficiaryPrepopulatedIndicator,
        PaymentPurposeCode = data.PaymentPurposeCode?.MapToPaymentPurposeCode(),
        CategoryPurposeCode = null, // not in v3
        BeneficiaryAccountType = data.BeneficiaryAccountType switch
        {
            VariableRecurringPaymentsModelsV3p1p11.OBExternalExtendedAccountType1Code.Business =>
                OBInternalExtendedAccountType1Code.Business,
            VariableRecurringPaymentsModelsV3p1p11.OBExternalExtendedAccountType1Code.BusinessSavingsAccount =>
                OBInternalExtendedAccountType1Code.BusinessSavingsAccount,
            VariableRecurringPaymentsModelsV3p1p11.OBExternalExtendedAccountType1Code.Charity =>
                OBInternalExtendedAccountType1Code.Charity,
            VariableRecurringPaymentsModelsV3p1p11.OBExternalExtendedAccountType1Code.Collection =>
                OBInternalExtendedAccountType1Code.Collection,
            VariableRecurringPaymentsModelsV3p1p11.OBExternalExtendedAccountType1Code.Corporate =>
                OBInternalExtendedAccountType1Code.Corporate,
            VariableRecurringPaymentsModelsV3p1p11.OBExternalExtendedAccountType1Code.Ewallet =>
                OBInternalExtendedAccountType1Code.Ewallet,
            VariableRecurringPaymentsModelsV3p1p11.OBExternalExtendedAccountType1Code.Government =>
                OBInternalExtendedAccountType1Code.Government,
            VariableRecurringPaymentsModelsV3p1p11.OBExternalExtendedAccountType1Code.Investment =>
                OBInternalExtendedAccountType1Code.Investment,
            VariableRecurringPaymentsModelsV3p1p11.OBExternalExtendedAccountType1Code.ISA =>
                OBInternalExtendedAccountType1Code
                    .ISA,
            VariableRecurringPaymentsModelsV3p1p11.OBExternalExtendedAccountType1Code.JointPersonal =>
                OBInternalExtendedAccountType1Code.JointPersonal,
            VariableRecurringPaymentsModelsV3p1p11.OBExternalExtendedAccountType1Code.Pension =>
                OBInternalExtendedAccountType1Code.Pension,
            VariableRecurringPaymentsModelsV3p1p11.OBExternalExtendedAccountType1Code.Personal =>
                OBInternalExtendedAccountType1Code.Personal,
            VariableRecurringPaymentsModelsV3p1p11.OBExternalExtendedAccountType1Code.PersonalSavingsAccount =>
                OBInternalExtendedAccountType1Code.PersonalSavingsAccount,
            VariableRecurringPaymentsModelsV3p1p11.OBExternalExtendedAccountType1Code.Premier =>
                OBInternalExtendedAccountType1Code.Premier,
            VariableRecurringPaymentsModelsV3p1p11.OBExternalExtendedAccountType1Code.Wealth =>
                OBInternalExtendedAccountType1Code.Wealth,
            null => null,
            _ => throw new ArgumentOutOfRangeException()
        },
        DeliveryAddress = data.DeliveryAddress?.MapToDeliveryAddress()
    };

    private static OBExternalPurpose1Code MapToPaymentPurposeCode(
        this string data)
    {
        // NB: invalid values cannot be POSTed so should not be returned
        if (!Enum.TryParse(data, out OBExternalPurpose1Code code))
        {
            throw new ArgumentOutOfRangeException($"Received unexpeted value: ${data}");
        }
        return code;
    }

    private static OBRisk1PaymentContextCode MapToPaymentContextCode(
        this VariableRecurringPaymentsModelsV3p1p11.OBRisk1PaymentContextCode data) =>
        // NB: invalid values cannot be POSTed so should not be returned
        data switch
        {
            VariableRecurringPaymentsModelsV3p1p11.OBRisk1PaymentContextCode.BillingGoodsAndServicesInAdvance =>
                OBRisk1PaymentContextCode.BillingGoodsAndServicesInAdvance,
            VariableRecurringPaymentsModelsV3p1p11.OBRisk1PaymentContextCode.BillingGoodsAndServicesInArrears =>
                OBRisk1PaymentContextCode.BillingGoodsAndServicesInArrears,
            VariableRecurringPaymentsModelsV3p1p11.OBRisk1PaymentContextCode.PispPayee =>
                throw new ArgumentOutOfRangeException($"Received unexpeted value: ${data}"),
            VariableRecurringPaymentsModelsV3p1p11.OBRisk1PaymentContextCode.EcommerceMerchantInitiatedPayment =>
                OBRisk1PaymentContextCode.EcommerceMerchantInitiatedPayment,
            VariableRecurringPaymentsModelsV3p1p11.OBRisk1PaymentContextCode.FaceToFacePointOfSale =>
                OBRisk1PaymentContextCode
                    .FaceToFacePointOfSale,
            VariableRecurringPaymentsModelsV3p1p11.OBRisk1PaymentContextCode.TransferToSelf => OBRisk1PaymentContextCode
                .TransferToSelf,
            VariableRecurringPaymentsModelsV3p1p11.OBRisk1PaymentContextCode.TransferToThirdParty =>
                OBRisk1PaymentContextCode
                    .TransferToThirdParty,
            VariableRecurringPaymentsModelsV3p1p11.OBRisk1PaymentContextCode.BillPayment =>
                throw new ArgumentOutOfRangeException($"Received unexpeted value: ${data}"),
            VariableRecurringPaymentsModelsV3p1p11.OBRisk1PaymentContextCode.EcommerceGoods =>
                throw new ArgumentOutOfRangeException($"Received unexpeted value: ${data}"),
            VariableRecurringPaymentsModelsV3p1p11.OBRisk1PaymentContextCode.EcommerceServices =>
                throw new ArgumentOutOfRangeException($"Received unexpeted value: ${data}"),
            VariableRecurringPaymentsModelsV3p1p11.OBRisk1PaymentContextCode.Other =>
                throw new ArgumentOutOfRangeException($"Received unexpeted value: ${data}"),
            VariableRecurringPaymentsModelsV3p1p11.OBRisk1PaymentContextCode.PartyToParty =>
                throw new ArgumentOutOfRangeException($"Received unexpeted value: ${data}"),
            _ => throw new ArgumentOutOfRangeException(nameof(data), data, null)
        };

    private static Links MapToLinks(
        this VariableRecurringPaymentsModelsV3p1p11.Links data) => new()
    {
        Self = data.Self,
        First = data.First,
        Prev = data.Prev,
        Next = data.Next,
        Last = data.Last,
        AdditionalProperties = data.AdditionalProperties
    };

    private static Meta MapToMeta(
        this VariableRecurringPaymentsModelsV3p1p11.Meta data) =>
        new() { AdditionalProperties = data.AdditionalProperties };

    private static DataReadRefundAccount MapToReadRefundAccount(
        this VariableRecurringPaymentsModelsV3p1p11.DataReadRefundAccount data) => data switch
    {
        VariableRecurringPaymentsModelsV3p1p11.DataReadRefundAccount.No => DataReadRefundAccount.No,
        VariableRecurringPaymentsModelsV3p1p11.DataReadRefundAccount.Yes => DataReadRefundAccount.Yes,
        _ => throw new ArgumentOutOfRangeException(nameof(data), data, null)
    };

    private static OBCashAccountDebtorWithName MapToDebtorAccount(
        this VariableRecurringPaymentsModelsV3p1p11.OBCashAccountDebtorWithName data) => new()
    {
        SchemeName = data.SchemeName,
        Identification = data.Identification,
        Name = data.Name,
        SecondaryIdentification = data.SecondaryIdentification,
        Proxy = null, // not in v3
        AdditionalProperties = data.AdditionalProperties
    };

    private static DebtorAccount MapToDebtorAccount2(
        this VariableRecurringPaymentsModelsV3p1p11.OBCashAccountDebtorWithName data) => new()
    {
        SchemeName = data.SchemeName,
        Identification = data.Identification,
        Name = data.Name,
        SecondaryIdentification = data.SecondaryIdentification,
        Proxy = null, // not in v3
        AdditionalProperties = data.AdditionalProperties
    };

    private static OBCashAccountCreditor3 MapToCreditorAccount(
        this VariableRecurringPaymentsModelsV3p1p11.OBCashAccountCreditor3 data) => new()
    {
        SchemeName = data.SchemeName,
        Identification = data.Identification,
        Name = data.Name,
        SecondaryIdentification = data.SecondaryIdentification,
        Proxy = null, // not in v3
        AdditionalProperties = data.AdditionalProperties
    };

    private static OBPostalAddress7 MapToCreditorPostalAddress(
        this VariableRecurringPaymentsModelsV3p1p11.OBPostalAddress6 data) => new()
    {
        AddressType = data.AddressType?.MapToAddressType(),
        Department = data.Department,
        SubDepartment = data.SubDepartment,
        StreetName = data.StreetName,
        BuildingNumber = data.BuildingNumber,
        BuildingName = null, // not in v3
        Floor = null, // not in v3
        UnitNumber = null, // not in v3
        Room = null, // not in v3
        PostBox = null, // not in v3
        TownLocationName = null, // not in v3
        DistrictName = null, // not in v3
        CareOf = null, // not in v3
        PostCode = data.PostCode,
        TownName = data.TownName,
        CountrySubDivision = data.CountrySubDivision,
        Country = data.Country,
        AddressLine = data.AddressLine
    };

    private static OBPostalAddress7 MapToDeliveryAddress(
        this VariableRecurringPaymentsModelsV3p1p11.DeliveryAddress data) => new()
    {
        AddressType = null, // not in v3
        Department = null, // not in v3
        SubDepartment = null, // not in v3
        StreetName = data.StreetName,
        BuildingNumber = data.BuildingNumber,
        BuildingName = null, // not in v3
        Floor = null, // not in v3
        UnitNumber = null, // not in v3
        Room = null, // not in v3
        PostBox = null, // not in v3
        TownLocationName = null, // not in v3
        DistrictName = null, // not in v3
        CareOf = null, // not in v3
        PostCode = data.PostCode,
        TownName = data.TownName,
        CountrySubDivision = data.CountrySubDivision,
        Country = data.Country,
        AddressLine = data.AddressLine
    };

    private static OBRemittanceInformation2 MapToRemittanceInformation(
        this VariableRecurringPaymentsModelsV3p1p11.RemittanceInformation data)
    {
        ICollection<OBRemittanceInformationStructured>? structured = null;
        ICollection<string>? unstructured = null;
        if (data.Reference is not null)
        {
            structured =
            [
                new OBRemittanceInformationStructured
                {
                    CreditorReferenceInformation = new CreditorReferenceInformation
                    {
                        Reference = data.Reference
                    }
                }
            ];
        }
        if (data.Unstructured is not null)
        {
            unstructured = [data.Unstructured];
        }
        return new OBRemittanceInformation2
        {
            Structured = structured,
            Unstructured = unstructured,
            AdditionalProperties = data.AdditionalProperties
        };
    }

    private static OBRemittanceInformation2 MapToRemittanceInformation2(
        this VariableRecurringPaymentsModelsV3p1p11.OBVRPRemittanceInformation data)
    {
        ICollection<OBRemittanceInformationStructured>? structured = null;
        ICollection<string>? unstructured = null;
        if (data.Reference is not null)
        {
            structured =
            [
                new OBRemittanceInformationStructured
                {
                    CreditorReferenceInformation = new CreditorReferenceInformation
                    {
                        Reference = data.Reference
                    }
                }
            ];
        }
        if (data.Unstructured is not null)
        {
            unstructured = [data.Unstructured];
        }
        return new OBRemittanceInformation2
        {
            Structured = structured,
            Unstructured = unstructured,
            AdditionalProperties = data.AdditionalProperties
        };
    }

    private static VariableRecurringPaymentsModelsV3p1p11.OBRisk1 MapFromRisk(this OBRisk1 risk) =>
        new()
        {
            PaymentContextCode = risk.PaymentContextCode switch
            {
                OBRisk1PaymentContextCode.BillingGoodsAndServicesInAdvance => VariableRecurringPaymentsModelsV3p1p11
                    .OBRisk1PaymentContextCode.BillingGoodsAndServicesInAdvance,
                OBRisk1PaymentContextCode.BillingGoodsAndServicesInArrears => VariableRecurringPaymentsModelsV3p1p11
                    .OBRisk1PaymentContextCode.BillingGoodsAndServicesInArrears,
                OBRisk1PaymentContextCode.EcommerceMerchantInitiatedPayment => VariableRecurringPaymentsModelsV3p1p11
                    .OBRisk1PaymentContextCode.EcommerceMerchantInitiatedPayment,
                OBRisk1PaymentContextCode.FaceToFacePointOfSale => VariableRecurringPaymentsModelsV3p1p11
                    .OBRisk1PaymentContextCode.FaceToFacePointOfSale,
                OBRisk1PaymentContextCode.TransferToSelf => VariableRecurringPaymentsModelsV3p1p11
                    .OBRisk1PaymentContextCode
                    .TransferToSelf,
                OBRisk1PaymentContextCode.TransferToThirdParty => VariableRecurringPaymentsModelsV3p1p11
                    .OBRisk1PaymentContextCode.TransferToThirdParty,
                null => null,
                _ => throw new ArgumentOutOfRangeException()
            },
            MerchantCategoryCode = risk.MerchantCategoryCode,
            MerchantCustomerIdentification = risk.MerchantCustomerIdentification,
            ContractPresentIndicator = risk.ContractPresentIndicator,
            ContractPresentInidicator = risk.ContractPresentInidicator,
            BeneficiaryPrepopulatedIndicator = risk.BeneficiaryPrepopulatedIndicator,
            PaymentPurposeCode = risk.PaymentPurposeCode?.ToString(),
            BeneficiaryAccountType = risk.BeneficiaryAccountType switch
            {
                OBInternalExtendedAccountType1Code.Business => VariableRecurringPaymentsModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Business,
                OBInternalExtendedAccountType1Code.BusinessSavingsAccount => VariableRecurringPaymentsModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.BusinessSavingsAccount,
                OBInternalExtendedAccountType1Code.Charity => VariableRecurringPaymentsModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Charity,
                OBInternalExtendedAccountType1Code.Collection => VariableRecurringPaymentsModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Collection,
                OBInternalExtendedAccountType1Code.Corporate => VariableRecurringPaymentsModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Corporate,
                OBInternalExtendedAccountType1Code.Ewallet => VariableRecurringPaymentsModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Ewallet,
                OBInternalExtendedAccountType1Code.Government => VariableRecurringPaymentsModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Government,
                OBInternalExtendedAccountType1Code.Investment => VariableRecurringPaymentsModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Investment,
                OBInternalExtendedAccountType1Code.ISA => VariableRecurringPaymentsModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.ISA,
                OBInternalExtendedAccountType1Code.JointPersonal => VariableRecurringPaymentsModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.JointPersonal,
                OBInternalExtendedAccountType1Code.Pension => VariableRecurringPaymentsModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Pension,
                OBInternalExtendedAccountType1Code.Personal => VariableRecurringPaymentsModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Personal,
                OBInternalExtendedAccountType1Code.PersonalSavingsAccount => VariableRecurringPaymentsModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.PersonalSavingsAccount,
                OBInternalExtendedAccountType1Code.Premier => VariableRecurringPaymentsModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Premier,
                OBInternalExtendedAccountType1Code.Wealth => VariableRecurringPaymentsModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Wealth,
                null => null,
                _ => throw new ArgumentOutOfRangeException()
            },
            DeliveryAddress = risk.DeliveryAddress?.MapFromDeliveryAddress()
        };

    private static VariableRecurringPaymentsModelsV3p1p11.OBVRPRemittanceInformation MapFromRemittanceInformation2(
        this OBRemittanceInformation2 data)
    {
        string? unstructured = null;
        if (data.Unstructured is not null &&
            data.Unstructured.Count > 0)
        {
            if (data.Unstructured.Count > 1)
            {
                throw new Exception("Only one unstructured element supported by V3 in RemittanceInformation");
            }
            unstructured = data.Unstructured.First();
        }

        string? reference = null;
        if (data.Structured is not null &&
            data.Structured.Count > 0)
        {
            if (data.Structured.Count > 1)
            {
                throw new Exception("Only one structured element supported by V3 in RemittanceInformation");
            }
            OBRemittanceInformationStructured structured = data.Structured.First();
            reference = structured.CreditorReferenceInformation?.Reference;
        }

        return new VariableRecurringPaymentsModelsV3p1p11.OBVRPRemittanceInformation
        {
            Unstructured = unstructured,
            Reference = reference,
            AdditionalProperties = data.AdditionalProperties
        };
    }

    private static VariableRecurringPaymentsModelsV3p1p11.RemittanceInformation MapFromRemittanceInformation(
        this OBRemittanceInformation2 data)
    {
        string? unstructured = null;
        if (data.Unstructured is not null &&
            data.Unstructured.Count > 0)
        {
            if (data.Unstructured.Count > 1)
            {
                throw new Exception("Only one unstructured element supported by V3 in RemittanceInformation");
            }
            unstructured = data.Unstructured.First();
        }

        string? reference = null;
        if (data.Structured is not null &&
            data.Structured.Count > 0)
        {
            if (data.Structured.Count > 1)
            {
                throw new Exception("Only one structured element supported by V3 in RemittanceInformation");
            }
            OBRemittanceInformationStructured structured = data.Structured.First();
            reference = structured.CreditorReferenceInformation?.Reference;
        }

        return new VariableRecurringPaymentsModelsV3p1p11.RemittanceInformation
        {
            Unstructured = unstructured,
            Reference = reference,
            AdditionalProperties = data.AdditionalProperties
        };
    }

    private static VariableRecurringPaymentsModelsV3p1p11.Data2ReadRefundAccount MapFromReadRefundAccount(
        this Data2ReadRefundAccount data) =>
        data switch
        {
            Data2ReadRefundAccount.No => VariableRecurringPaymentsModelsV3p1p11.Data2ReadRefundAccount.No,
            Data2ReadRefundAccount.Yes => VariableRecurringPaymentsModelsV3p1p11.Data2ReadRefundAccount.Yes,
            _ => throw new ArgumentOutOfRangeException(nameof(data), data, null)
        };

    private static VariableRecurringPaymentsModelsV3p1p11.OBCashAccountDebtorWithName MapFromDebtorAccount2(
        this OBCashAccountDebtorWithName data) => new()
    {
        SchemeName = data.SchemeName,
        Identification = data.Identification,
        Name = data.Name,
        SecondaryIdentification = data.SecondaryIdentification,
        AdditionalProperties = data.AdditionalProperties
    };

    private static VariableRecurringPaymentsModelsV3p1p11.OBCashAccountCreditor3 MapFromCreditorAccount(
        this OBCashAccountCreditor3 data) => new()
    {
        SchemeName = data.SchemeName,
        Identification = data.Identification,
        Name = data.Name,
        SecondaryIdentification = data.SecondaryIdentification,
        AdditionalProperties = data.AdditionalProperties
    };

    private static VariableRecurringPaymentsModelsV3p1p11.DebtorAccount MapFromDebtorAccount(
        this DebtorAccount data) => new()
    {
        SchemeName = data.SchemeName,
        Identification = data.Identification,
        Name = data.Name,
        SecondaryIdentification = data.SecondaryIdentification,
        AdditionalProperties = data.AdditionalProperties
    };

    private static VariableRecurringPaymentsModelsV3p1p11.OBPostalAddress6 MapFromCreditorPostalAddress(
        this OBPostalAddress7 data) => new()
    {
        AddressType = data.AddressType?.MapFromAddressType(),
        Department = data.Department,
        SubDepartment = data.SubDepartment,
        StreetName = data.StreetName,
        BuildingNumber = data.BuildingNumber,
        PostCode = data.PostCode,
        TownName = data.TownName,
        CountrySubDivision = data.CountrySubDivision,
        Country = data.Country,
        AddressLine = data.AddressLine
    };

    private static VariableRecurringPaymentsModelsV3p1p11.DeliveryAddress MapFromDeliveryAddress(
        this OBPostalAddress7 data) => new()
    {
        StreetName = data.StreetName,
        BuildingNumber = data.BuildingNumber,
        PostCode = data.PostCode,
        TownName = data.TownName ?? throw new Exception("TownName is required in DeliveryAddress"),
        CountrySubDivision = data.CountrySubDivision,
        Country = data.Country ?? throw new Exception("Country is required in DeliveryAddress"),
        AddressLine = data.AddressLine
        //AdditionalProperties
    };

    private static VariableRecurringPaymentsModelsV3p1p11.OBAddressTypeCode MapFromAddressType(
        this OBAddressType2Code addressType) =>
        addressType switch
        {
            OBAddressType2Code.BIZZ => VariableRecurringPaymentsModelsV3p1p11.OBAddressTypeCode.Business,
            OBAddressType2Code.DLVY => VariableRecurringPaymentsModelsV3p1p11.OBAddressTypeCode.DeliveryTo,
            OBAddressType2Code.MLTO => VariableRecurringPaymentsModelsV3p1p11.OBAddressTypeCode.MailTo,
            OBAddressType2Code.PBOX => VariableRecurringPaymentsModelsV3p1p11.OBAddressTypeCode.POBox,
            OBAddressType2Code.ADDR => VariableRecurringPaymentsModelsV3p1p11.OBAddressTypeCode.Postal,
            OBAddressType2Code.HOME => VariableRecurringPaymentsModelsV3p1p11.OBAddressTypeCode.Residential,
            OBAddressType2Code.CORR => VariableRecurringPaymentsModelsV3p1p11.OBAddressTypeCode.Correspondence,
            OBAddressType2Code.STAT => VariableRecurringPaymentsModelsV3p1p11.OBAddressTypeCode.Statement,
            _ => throw new ArgumentOutOfRangeException(nameof(addressType), addressType, null)
        };

    private static OBAddressType2Code MapToAddressType(
        this VariableRecurringPaymentsModelsV3p1p11.OBAddressTypeCode addressType) =>
        addressType switch
        {
            VariableRecurringPaymentsModelsV3p1p11.OBAddressTypeCode.Business => OBAddressType2Code.BIZZ,
            VariableRecurringPaymentsModelsV3p1p11.OBAddressTypeCode.Correspondence => OBAddressType2Code.CORR,
            VariableRecurringPaymentsModelsV3p1p11.OBAddressTypeCode.DeliveryTo => OBAddressType2Code.DLVY,
            VariableRecurringPaymentsModelsV3p1p11.OBAddressTypeCode.MailTo => OBAddressType2Code.MLTO,
            VariableRecurringPaymentsModelsV3p1p11.OBAddressTypeCode.POBox => OBAddressType2Code.PBOX,
            VariableRecurringPaymentsModelsV3p1p11.OBAddressTypeCode.Postal => OBAddressType2Code.ADDR,
            VariableRecurringPaymentsModelsV3p1p11.OBAddressTypeCode.Residential => OBAddressType2Code.HOME,
            VariableRecurringPaymentsModelsV3p1p11.OBAddressTypeCode.Statement => OBAddressType2Code.STAT,
            _ => throw new ArgumentOutOfRangeException(nameof(addressType), addressType, null)
        };
}
