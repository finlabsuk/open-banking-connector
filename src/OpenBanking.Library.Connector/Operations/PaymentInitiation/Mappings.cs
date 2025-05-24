// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// using PaymentInitiationModelsV3p1p11 =
//     FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.NSwagPisp.Models;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V4p0.NSwagPisp.Models;

public static class Mappings
{
    public static PaymentInitiationModelsV3p1p11.OBWriteDomesticConsent4 MapFromOBWriteDomesticConsent4(
        OBWriteDomesticConsent4 externalApiRequest) =>
        new()
        {
            Data = new PaymentInitiationModelsV3p1p11.Data2
            {
                ReadRefundAccount = externalApiRequest.Data.ReadRefundAccount?.MapFromReadRefundAccount(),
                Initiation = new PaymentInitiationModelsV3p1p11.Initiation2
                {
                    InstructionIdentification =
                        externalApiRequest.Data.Initiation.InstructionIdentification,
                    EndToEndIdentification = externalApiRequest.Data.Initiation.EndToEndIdentification,
                    LocalInstrument = externalApiRequest.Data.Initiation.LocalInstrument,
                    InstructedAmount =
                        new PaymentInitiationModelsV3p1p11.InstructedAmount2
                        {
                            Amount = externalApiRequest.Data.Initiation.InstructedAmount.Amount,
                            Currency = externalApiRequest.Data.Initiation.InstructedAmount.Currency
                        },
                    DebtorAccount =
                        externalApiRequest.Data.Initiation.DebtorAccount?.MapFromDebtorAccount2(),
                    CreditorAccount =
                        new PaymentInitiationModelsV3p1p11.CreditorAccount2
                        {
                            SchemeName = externalApiRequest.Data.Initiation.CreditorAccount.SchemeName,
                            Identification = externalApiRequest.Data.Initiation.CreditorAccount.Identification,
                            Name = externalApiRequest.Data.Initiation.CreditorAccount.Name,
                            SecondaryIdentification =
                                externalApiRequest.Data.Initiation.CreditorAccount.SecondaryIdentification
                        },
                    CreditorPostalAddress =
                        externalApiRequest.Data.Initiation.CreditorPostalAddress?.MapFromCreditorPostalAddress(),
                    RemittanceInformation =
                        externalApiRequest.Data.Initiation.RemittanceInformation?.MapFromRemittanceInformation2(),
                    SupplementaryData = externalApiRequest.Data.Initiation.SupplementaryData?.MapFromSupplementaryData()
                },
                Authorisation = externalApiRequest.Data.Authorisation?.MapFromAuthorisation(),
                SCASupportData = externalApiRequest.Data.SCASupportData?.MapFromSCASupportData()
            },
            Risk = externalApiRequest.Risk.MapFromRisk()
        };

    public static PaymentInitiationModelsV3p1p11.OBWriteDomestic2 MapFromOBWriteDomestic2(
        OBWriteDomestic2 externalApiRequest) => new()
    {
        Data = new PaymentInitiationModelsV3p1p11.Data
        {
            ConsentId = externalApiRequest.Data.ConsentId,
            Initiation = new PaymentInitiationModelsV3p1p11.Initiation
            {
                InstructionIdentification = externalApiRequest.Data.Initiation.InstructionIdentification,
                EndToEndIdentification = externalApiRequest.Data.Initiation.EndToEndIdentification,
                LocalInstrument = externalApiRequest.Data.Initiation.LocalInstrument,
                InstructedAmount =
                    new PaymentInitiationModelsV3p1p11.InstructedAmount
                    {
                        Amount = externalApiRequest.Data.Initiation.InstructedAmount.Amount,
                        Currency = externalApiRequest.Data.Initiation.InstructedAmount.Currency
                    },
                DebtorAccount =
                    externalApiRequest.Data.Initiation.DebtorAccount?.MapFromDebtorAccount(),
                CreditorAccount =
                    new PaymentInitiationModelsV3p1p11.CreditorAccount
                    {
                        SchemeName = externalApiRequest.Data.Initiation.CreditorAccount.SchemeName,
                        Identification = externalApiRequest.Data.Initiation.CreditorAccount.Identification,
                        Name = externalApiRequest.Data.Initiation.CreditorAccount.Name,
                        SecondaryIdentification =
                            externalApiRequest.Data.Initiation.CreditorAccount.SecondaryIdentification
                    },
                CreditorPostalAddress =
                    externalApiRequest.Data.Initiation.CreditorPostalAddress?.MapFromCreditorPostalAddress(),
                RemittanceInformation =
                    externalApiRequest.Data.Initiation.RemittanceInformation?.MapFromRemittanceInformation(),
                SupplementaryData = externalApiRequest.Data.Initiation.SupplementaryData?.MapFromSupplementaryData()
            }
        },
        Risk = externalApiRequest.Risk.MapFromRisk()
    };

    public static OBWriteDomesticConsentResponse5 MapToOBWriteDomesticConsentResponse5(
        PaymentInitiationModelsV3p1p11.OBWriteDomesticConsentResponse5 externalApiResponseV3) =>
        new()
        {
            Data = new Data3
            {
                ConsentId = externalApiResponseV3.Data.ConsentId,
                CreationDateTime = externalApiResponseV3.Data.CreationDateTime,
                Status = externalApiResponseV3.Data.Status.MapToStatus(),
                StatusReason = null, // not in v3
                StatusUpdateDateTime = externalApiResponseV3.Data.StatusUpdateDateTime,
                ReadRefundAccount = externalApiResponseV3.Data.ReadRefundAccount?.MapToReadRefundAccount(),
                CutOffDateTime = externalApiResponseV3.Data.CutOffDateTime,
                ExpectedExecutionDateTime = externalApiResponseV3.Data.ExpectedExecutionDateTime,
                ExpectedSettlementDateTime = externalApiResponseV3.Data.ExpectedSettlementDateTime,
                Charges = externalApiResponseV3.Data.Charges?
                    .Select(a => a.MapToCharges())
                    .ToList(),
                Initiation = new Initiation3
                {
                    InstructionIdentification = externalApiResponseV3.Data.Initiation.InstructionIdentification,
                    EndToEndIdentification = externalApiResponseV3.Data.Initiation.EndToEndIdentification,
                    LocalInstrument = externalApiResponseV3.Data.Initiation.LocalInstrument,
                    InstructedAmount = externalApiResponseV3.Data.Initiation.InstructedAmount.MapToInstructedAmount(),
                    DebtorAccount = externalApiResponseV3.Data.Initiation.DebtorAccount?.MapToDebtorAccount(),
                    CreditorAgent = null, // not in v3
                    CreditorAccount = externalApiResponseV3.Data.Initiation.CreditorAccount.MapToCreditorAccount(),
                    CreditorPostalAddress =
                        externalApiResponseV3.Data.Initiation.CreditorPostalAddress?.MapToCreditorPostalAddress(),
                    UltimateCreditor = null, // not in v3
                    UltimateDebtor = null, // not in v3
                    RegulatoryReporting = null, // not in v3
                    RemittanceInformation =
                        externalApiResponseV3.Data.Initiation.RemittanceInformation?.MapToRemittanceInformation(),
                    SupplementaryData =
                        externalApiResponseV3.Data.Initiation.SupplementaryData?.MapToSupplementaryData()
                },
                Authorisation = externalApiResponseV3.Data.Authorisation?.MapToAuthorisation(),
                SCASupportData = externalApiResponseV3.Data.SCASupportData?.MapToSCASupportData(),
                Debtor = externalApiResponseV3.Data.Debtor?.MapToDebtor()
            },
            Risk = externalApiResponseV3.Risk.MapToRisk(),
            Links = externalApiResponseV3.Links?.MapToLinks(),
            Meta = externalApiResponseV3.Meta?.MapToMeta()
        };

    public static OBWriteDomesticResponse5 MapToOBWriteDomesticResponse5(
        PaymentInitiationModelsV3p1p11.OBWriteDomesticResponse5 externalApiResponseV3) =>
        new()
        {
            Data = new Data4
            {
                DomesticPaymentId = externalApiResponseV3.Data.DomesticPaymentId,
                ConsentId = externalApiResponseV3.Data.ConsentId,
                CreationDateTime = externalApiResponseV3.Data.CreationDateTime,
                Status = externalApiResponseV3.Data.Status.MapToStatus2(),
                StatusUpdateDateTime = externalApiResponseV3.Data.StatusUpdateDateTime,
                StatusReason = null, // not in v3
                ExpectedExecutionDateTime = externalApiResponseV3.Data.ExpectedExecutionDateTime,
                ExpectedSettlementDateTime = externalApiResponseV3.Data.ExpectedSettlementDateTime,
                Refund = externalApiResponseV3.Data.Refund?.MapToRefund(),
                Charges = externalApiResponseV3.Data.Charges?
                    .Select(a => a.MapToCharges2())
                    .ToList(),
                Initiation = new Initiation4
                {
                    InstructionIdentification = externalApiResponseV3.Data.Initiation.InstructionIdentification,
                    EndToEndIdentification = externalApiResponseV3.Data.Initiation.EndToEndIdentification,
                    LocalInstrument = externalApiResponseV3.Data.Initiation.LocalInstrument,
                    InstructedAmount = externalApiResponseV3.Data.Initiation.InstructedAmount.MapToInstructedAmount4(),
                    DebtorAccount = externalApiResponseV3.Data.Initiation.DebtorAccount?.MapToDebtorAccount4(),
                    CreditorAgent = null, // not in v3
                    CreditorAccount = externalApiResponseV3.Data.Initiation.CreditorAccount.MapToCreditorAccount4(),
                    CreditorPostalAddress =
                        externalApiResponseV3.Data.Initiation.CreditorPostalAddress?.MapToCreditorPostalAddress(),
                    UltimateCreditor = null, // not in v3
                    UltimateDebtor = null, // not in v3
                    RegulatoryReporting = null, // not in v3
                    RemittanceInformation =
                        externalApiResponseV3.Data.Initiation.RemittanceInformation?.MapToRemittanceInformation2(),
                    SupplementaryData =
                        externalApiResponseV3.Data.Initiation.SupplementaryData?.MapToSupplementaryData()
                },
                MultiAuthorisation = externalApiResponseV3.Data.MultiAuthorisation?.MapToMultiAuthorisation(),
                Debtor = externalApiResponseV3.Data.Debtor?.MapToDebtor()
            },
            Links = externalApiResponseV3.Links?.MapToLinks(),
            Meta = externalApiResponseV3.Meta?.MapToMeta()
        };

    public static OBWriteFundsConfirmationResponse1 MapToOBWriteFundsConfirmationResponse1(
        PaymentInitiationModelsV3p1p11.OBWriteFundsConfirmationResponse1 externalApiResponseV3) =>
        new()
        {
            Data = new Data17
            {
                FundsAvailableResult =
                    externalApiResponseV3.Data.FundsAvailableResult?.MapToFundsAvailableResult(),
                SupplementaryData = externalApiResponseV3.Data.SupplementaryData?.MapToSupplementaryData()
            },
            Links = externalApiResponseV3.Links?.MapToLinks(),
            Meta = externalApiResponseV3.Meta?.MapToMeta()
        };

    public static OBWritePaymentDetailsResponse1 MapToOBWritePaymentDetailsResponse1(
        PaymentInitiationModelsV3p1p11.OBWritePaymentDetailsResponse1 externalApiResponseV3) =>
        new()
        {
            Data = new Data30
            {
                PaymentStatus = externalApiResponseV3.Data.PaymentStatus?
                    .Select(a => a.MapToPaymentStatus())
                    .ToList()
            },
            Links = externalApiResponseV3.Links?.MapToLinks(),
            Meta = externalApiResponseV3.Meta?.MapToMeta()
            //AdditionalProperties
        };

    private static FundsAvailableResult MapToFundsAvailableResult(
        this PaymentInitiationModelsV3p1p11.FundsAvailableResult data) => new()
    {
        FundsAvailableDateTime = data.FundsAvailableDateTime,
        FundsAvailable = data.FundsAvailable
    };

    private static OBWritePaymentDetails1 MapToPaymentStatus(
        this PaymentInitiationModelsV3p1p11.PaymentStatus data) => new()
    {
        PaymentTransactionId = data.PaymentTransactionId,
        Status = OBWritePaymentDetails1Status.PDNG, // decision: use PDNG (pending) as placeholder for required field
        V3Status =
            data.Status, // decision: map Status to V3Status to avoid information loss converting between two different enums
        StatusUpdateDateTime = data.StatusUpdateDateTime,
        StatusDetail = data.StatusDetail?.MapToStatusDetail()
        //AdditionalProperties
    };

    private static StatusDetail MapToStatusDetail(
        this PaymentInitiationModelsV3p1p11.StatusDetail data) => new()
    {
        LocalInstrument = data.LocalInstrument,
        Status = StatusDetailStatus.PDNG, // decision: use PDNG (pending) as placeholder for required field
        V3Status = data.Status, // decision: map Status to V3Status to avoid information loss converting string to enum
        StatusReason = null, // decision: placeholder
        V3StatusReason =
            data.StatusReason, // decision: map StatusReason to V3StatusReason to avoid information loss converting between two different enums
        StatusReasonDescription = data.StatusReasonDescription
        //AdditionalProperties
    };

    private static MultiAuthorisation MapToMultiAuthorisation(
        this PaymentInitiationModelsV3p1p11.MultiAuthorisation data) => new()
    {
        Status = data.Status switch
        {
            PaymentInitiationModelsV3p1p11.MultiAuthorisationStatus.Authorised => MultiAuthorisationStatus.AUTH,
            PaymentInitiationModelsV3p1p11.MultiAuthorisationStatus.AwaitingFurtherAuthorisation =>
                MultiAuthorisationStatus.AWAF,
            PaymentInitiationModelsV3p1p11.MultiAuthorisationStatus.Rejected => MultiAuthorisationStatus.RJCT,
            _ => throw new ArgumentOutOfRangeException()
        },
        NumberRequired = data.NumberRequired,
        NumberReceived = data.NumberReceived,
        LastUpdateDateTime = data.LastUpdateDateTime,
        ExpirationDateTime = data.ExpirationDateTime
    };

    private static Data3Status MapToStatus(this PaymentInitiationModelsV3p1p11.Data3Status data) =>
        data switch
        {
            PaymentInitiationModelsV3p1p11.Data3Status.Authorised => Data3Status.AUTH,
            PaymentInitiationModelsV3p1p11.Data3Status.AwaitingAuthorisation => Data3Status.AWAU,
            PaymentInitiationModelsV3p1p11.Data3Status.Consumed => Data3Status.COND,
            PaymentInitiationModelsV3p1p11.Data3Status.Rejected => Data3Status.RJCT,
            _ => throw new ArgumentOutOfRangeException()
        };

    private static Data4Status MapToStatus2(this PaymentInitiationModelsV3p1p11.Data4Status data) =>
        data switch
        {
            PaymentInitiationModelsV3p1p11.Data4Status.AcceptedCreditSettlementCompleted => Data4Status.ACCC,
            PaymentInitiationModelsV3p1p11.Data4Status.AcceptedSettlementCompleted => Data4Status.ACSC,
            PaymentInitiationModelsV3p1p11.Data4Status.AcceptedSettlementInProcess => Data4Status.ACSP,
            PaymentInitiationModelsV3p1p11.Data4Status.AcceptedWithoutPosting => Data4Status.ACWP,
            PaymentInitiationModelsV3p1p11.Data4Status.Pending => Data4Status.PDNG,
            PaymentInitiationModelsV3p1p11.Data4Status.Rejected => Data4Status.RJCT,
            _ => throw new ArgumentOutOfRangeException(nameof(data), data, null)
        };

    private static Charges MapToCharges(
        this PaymentInitiationModelsV3p1p11.Charges data) => new()
    {
        ChargeBearer = data.ChargeBearer.MapToChargeBearer(),
        Type = data.Type,
        Amount = data.Amount.MapToAmount()
    };

    private static Charges2 MapToCharges2(
        this PaymentInitiationModelsV3p1p11.Charges2 data) => new()
    {
        ChargeBearer = data.ChargeBearer.MapToChargeBearer(),
        Type = data.Type,
        Amount = data.Amount.MapToAmount()
    };

    private static OBInternalChargeBearerType1Code MapToChargeBearer(
        this PaymentInitiationModelsV3p1p11.OBChargeBearerType1Code data) =>
        data switch
        {
            PaymentInitiationModelsV3p1p11.OBChargeBearerType1Code.BorneByCreditor => OBInternalChargeBearerType1Code
                .BorneByCreditor,
            PaymentInitiationModelsV3p1p11.OBChargeBearerType1Code.BorneByDebtor => OBInternalChargeBearerType1Code
                .BorneByDebtor,
            PaymentInitiationModelsV3p1p11.OBChargeBearerType1Code.FollowingServiceLevel =>
                OBInternalChargeBearerType1Code.FollowingServiceLevel,
            PaymentInitiationModelsV3p1p11.OBChargeBearerType1Code.Shared => OBInternalChargeBearerType1Code.Shared,
            _ => throw new ArgumentOutOfRangeException()
        };

    private static OBActiveOrHistoricCurrencyAndAmount MapToAmount(
        this PaymentInitiationModelsV3p1p11.OBActiveOrHistoricCurrencyAndAmount data) => new()
    {
        Amount = data.Amount,
        Currency = data.Currency
    };

    private static Authorisation2 MapToAuthorisation(
        this PaymentInitiationModelsV3p1p11.Authorisation2 data) => new()
    {
        AuthorisationType = data.AuthorisationType switch
        {
            PaymentInitiationModelsV3p1p11.Authorisation2AuthorisationType.Any => Authorisation2AuthorisationType.Any,
            PaymentInitiationModelsV3p1p11.Authorisation2AuthorisationType.Single => Authorisation2AuthorisationType
                .Single,
            null => null,
            _ => throw new ArgumentOutOfRangeException()
        },
        CompletionDateTime = data.CompletionDateTime
    };

    private static OBSCASupportData1 MapToSCASupportData(
        this PaymentInitiationModelsV3p1p11.OBSCASupportData1 data) => new()
    {
        RequestedSCAExemptionType = data.RequestedSCAExemptionType switch
        {
            PaymentInitiationModelsV3p1p11.OBSCASupportData1RequestedSCAExemptionType.BillPayment =>
                OBSCASupportData1RequestedSCAExemptionType.BillPayment,
            PaymentInitiationModelsV3p1p11.OBSCASupportData1RequestedSCAExemptionType.ContactlessTravel =>
                OBSCASupportData1RequestedSCAExemptionType.ContactlessTravel,
            PaymentInitiationModelsV3p1p11.OBSCASupportData1RequestedSCAExemptionType.EcommerceGoods =>
                OBSCASupportData1RequestedSCAExemptionType.EcommerceGoods,
            PaymentInitiationModelsV3p1p11.OBSCASupportData1RequestedSCAExemptionType.EcommerceServices =>
                OBSCASupportData1RequestedSCAExemptionType.EcommerceServices,
            PaymentInitiationModelsV3p1p11.OBSCASupportData1RequestedSCAExemptionType.Kiosk =>
                OBSCASupportData1RequestedSCAExemptionType.Kiosk,
            PaymentInitiationModelsV3p1p11.OBSCASupportData1RequestedSCAExemptionType.Parking =>
                OBSCASupportData1RequestedSCAExemptionType.Parking,
            PaymentInitiationModelsV3p1p11.OBSCASupportData1RequestedSCAExemptionType.PartyToParty =>
                OBSCASupportData1RequestedSCAExemptionType.PartyToParty,
            null => null,
            _ => throw new ArgumentOutOfRangeException()
        },
        AppliedAuthenticationApproach = data.AppliedAuthenticationApproach switch
        {
            PaymentInitiationModelsV3p1p11.OBSCASupportData1AppliedAuthenticationApproach.CA =>
                OBSCASupportData1AppliedAuthenticationApproach.CA,
            PaymentInitiationModelsV3p1p11.OBSCASupportData1AppliedAuthenticationApproach.SCA =>
                OBSCASupportData1AppliedAuthenticationApproach.SCA,
            null => null,
            _ => throw new ArgumentOutOfRangeException()
        },
        ReferencePaymentOrderId = data.ReferencePaymentOrderId,
        AdditionalProperties = data.AdditionalProperties
    };

    private static OBCashAccountDebtor4 MapToDebtor(
        this PaymentInitiationModelsV3p1p11.OBCashAccountDebtor4 data) => new()
    {
        SchemeName = data.SchemeName,
        Identification = data.Identification,
        Name = data.Name,
        SecondaryIdentification = data.SecondaryIdentification,
        LEI = null, // not in v3
        AdditionalProperties = data.AdditionalProperties
    };

    private static OBRisk1 MapToRisk(
        this PaymentInitiationModelsV3p1p11.OBRisk1 data) => new()
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
            PaymentInitiationModelsV3p1p11.OBExternalExtendedAccountType1Code.Business =>
                OBInternalExtendedAccountType1Code.Business,
            PaymentInitiationModelsV3p1p11.OBExternalExtendedAccountType1Code.BusinessSavingsAccount =>
                OBInternalExtendedAccountType1Code.BusinessSavingsAccount,
            PaymentInitiationModelsV3p1p11.OBExternalExtendedAccountType1Code.Charity =>
                OBInternalExtendedAccountType1Code.Charity,
            PaymentInitiationModelsV3p1p11.OBExternalExtendedAccountType1Code.Collection =>
                OBInternalExtendedAccountType1Code.Collection,
            PaymentInitiationModelsV3p1p11.OBExternalExtendedAccountType1Code.Corporate =>
                OBInternalExtendedAccountType1Code.Corporate,
            PaymentInitiationModelsV3p1p11.OBExternalExtendedAccountType1Code.Ewallet =>
                OBInternalExtendedAccountType1Code.Ewallet,
            PaymentInitiationModelsV3p1p11.OBExternalExtendedAccountType1Code.Government =>
                OBInternalExtendedAccountType1Code.Government,
            PaymentInitiationModelsV3p1p11.OBExternalExtendedAccountType1Code.Investment =>
                OBInternalExtendedAccountType1Code.Investment,
            PaymentInitiationModelsV3p1p11.OBExternalExtendedAccountType1Code.ISA => OBInternalExtendedAccountType1Code
                .ISA,
            PaymentInitiationModelsV3p1p11.OBExternalExtendedAccountType1Code.JointPersonal =>
                OBInternalExtendedAccountType1Code.JointPersonal,
            PaymentInitiationModelsV3p1p11.OBExternalExtendedAccountType1Code.Pension =>
                OBInternalExtendedAccountType1Code.Pension,
            PaymentInitiationModelsV3p1p11.OBExternalExtendedAccountType1Code.Personal =>
                OBInternalExtendedAccountType1Code.Personal,
            PaymentInitiationModelsV3p1p11.OBExternalExtendedAccountType1Code.PersonalSavingsAccount =>
                OBInternalExtendedAccountType1Code.PersonalSavingsAccount,
            PaymentInitiationModelsV3p1p11.OBExternalExtendedAccountType1Code.Premier =>
                OBInternalExtendedAccountType1Code.Premier,
            PaymentInitiationModelsV3p1p11.OBExternalExtendedAccountType1Code.Wealth =>
                OBInternalExtendedAccountType1Code.Wealth,
            null => null,
            _ => throw new ArgumentOutOfRangeException()
        },
        DeliveryAddress = data.DeliveryAddress?.MapToDeliveryAddress()
    };

    private static ExternalPurpose1Code MapToPaymentPurposeCode(
        this string data)
    {
        // NB: invalid values cannot be POSTed so should not be returned
        if (!Enum.TryParse(data, out ExternalPurpose1Code code))
        {
            throw new ArgumentOutOfRangeException($"Received unexpeted value: ${data}");
        }
        return code;
    }

    private static OBRisk1PaymentContextCode MapToPaymentContextCode(
        this PaymentInitiationModelsV3p1p11.OBRisk1PaymentContextCode data) =>
        // NB: invalid values cannot be POSTed so should not be returned
        data switch
        {
            PaymentInitiationModelsV3p1p11.OBRisk1PaymentContextCode.BillingGoodsAndServicesInAdvance =>
                OBRisk1PaymentContextCode.BillingGoodsAndServicesInAdvance,
            PaymentInitiationModelsV3p1p11.OBRisk1PaymentContextCode.BillingGoodsAndServicesInArrears =>
                OBRisk1PaymentContextCode.BillingGoodsAndServicesInArrears,
            PaymentInitiationModelsV3p1p11.OBRisk1PaymentContextCode.PispPayee => throw new ArgumentOutOfRangeException(
                $"Received unexpeted value: ${data}"),
            PaymentInitiationModelsV3p1p11.OBRisk1PaymentContextCode.EcommerceMerchantInitiatedPayment =>
                OBRisk1PaymentContextCode.EcommerceMerchantInitiatedPayment,
            PaymentInitiationModelsV3p1p11.OBRisk1PaymentContextCode.FaceToFacePointOfSale => OBRisk1PaymentContextCode
                .FaceToFacePointOfSale,
            PaymentInitiationModelsV3p1p11.OBRisk1PaymentContextCode.TransferToSelf => OBRisk1PaymentContextCode
                .TransferToSelf,
            PaymentInitiationModelsV3p1p11.OBRisk1PaymentContextCode.TransferToThirdParty => OBRisk1PaymentContextCode
                .TransferToThirdParty,
            PaymentInitiationModelsV3p1p11.OBRisk1PaymentContextCode.BillPayment =>
                throw new ArgumentOutOfRangeException($"Received unexpeted value: ${data}"),
            PaymentInitiationModelsV3p1p11.OBRisk1PaymentContextCode.EcommerceGoods =>
                throw new ArgumentOutOfRangeException($"Received unexpeted value: ${data}"),
            PaymentInitiationModelsV3p1p11.OBRisk1PaymentContextCode.EcommerceServices =>
                throw new ArgumentOutOfRangeException($"Received unexpeted value: ${data}"),
            PaymentInitiationModelsV3p1p11.OBRisk1PaymentContextCode.Other => throw new ArgumentOutOfRangeException(
                $"Received unexpeted value: ${data}"),
            PaymentInitiationModelsV3p1p11.OBRisk1PaymentContextCode.PartyToParty =>
                throw new ArgumentOutOfRangeException($"Received unexpeted value: ${data}"),
            _ => throw new ArgumentOutOfRangeException(nameof(data), data, null)
        };

    private static Links MapToLinks(
        this PaymentInitiationModelsV3p1p11.Links data) => new()
    {
        Self = data.Self,
        First = data.First,
        Prev = data.Prev,
        Next = data.Next,
        Last = data.Last
    };

    private static Meta MapToMeta(
        this PaymentInitiationModelsV3p1p11.Meta data) => new()
    {
        TotalPages = data.TotalPages,
        FirstAvailableDateTime = data.FirstAvailableDateTime,
        LastAvailableDateTime = data.LastAvailableDateTime
    };

    private static OBSupplementaryData1 MapToSupplementaryData(
        this PaymentInitiationModelsV3p1p11.OBSupplementaryData1 data) =>
        new() { AdditionalProperties = data.AdditionalProperties };


    private static Data3ReadRefundAccount MapToReadRefundAccount(
        this PaymentInitiationModelsV3p1p11.Data3ReadRefundAccount data) => data switch
    {
        PaymentInitiationModelsV3p1p11.Data3ReadRefundAccount.No => Data3ReadRefundAccount.No,
        PaymentInitiationModelsV3p1p11.Data3ReadRefundAccount.Yes => Data3ReadRefundAccount.Yes,
        _ => throw new ArgumentOutOfRangeException(nameof(data), data, null)
    };

    private static InstructedAmount3 MapToInstructedAmount(
        this PaymentInitiationModelsV3p1p11.InstructedAmount3 data) => new()
    {
        Amount = data.Amount,
        Currency = data.Currency
    };

    private static InstructedAmount4 MapToInstructedAmount4(
        this PaymentInitiationModelsV3p1p11.InstructedAmount4 data) => new()
    {
        Amount = data.Amount,
        Currency = data.Currency
    };

    private static DebtorAccount3 MapToDebtorAccount(
        this PaymentInitiationModelsV3p1p11.DebtorAccount3 data) => new()
    {
        SchemeName = data.SchemeName,
        Identification = data.Identification,
        Name = data.Name,
        SecondaryIdentification = data.SecondaryIdentification,
        Proxy = null // not in v3
    };

    private static DebtorAccount4 MapToDebtorAccount4(
        this PaymentInitiationModelsV3p1p11.DebtorAccount4 data) => new()
    {
        SchemeName = data.SchemeName,
        Identification = data.Identification,
        Name = data.Name,
        SecondaryIdentification = data.SecondaryIdentification,
        Proxy = null // not in v3
    };

    private static Refund MapToRefund(
        this PaymentInitiationModelsV3p1p11.Refund data) => new() { Account = data.Account.MapToAccount() };

    private static Account MapToAccount(
        this PaymentInitiationModelsV3p1p11.Account data) => new()
    {
        SchemeName = data.SchemeName,
        Identification = data.Identification,
        Name = data.Name,
        SecondaryIdentification = data.SecondaryIdentification
    };

    private static CreditorAccount3 MapToCreditorAccount(
        this PaymentInitiationModelsV3p1p11.CreditorAccount3 data) => new()
    {
        SchemeName = data.SchemeName,
        Identification = data.Identification,
        Name = data.Name,
        SecondaryIdentification = data.SecondaryIdentification,
        Proxy = null // not in v3
    };

    private static CreditorAccount4 MapToCreditorAccount4(
        this PaymentInitiationModelsV3p1p11.CreditorAccount4 data) => new()
    {
        SchemeName = data.SchemeName,
        Identification = data.Identification,
        Name = data.Name,
        SecondaryIdentification = data.SecondaryIdentification,
        Proxy = null // not in v3
    };

    private static OBPostalAddress7 MapToCreditorPostalAddress(
        this PaymentInitiationModelsV3p1p11.OBPostalAddress6 data) => new()
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
        this PaymentInitiationModelsV3p1p11.DeliveryAddress data) => new()
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
        this PaymentInitiationModelsV3p1p11.RemittanceInformation3 data)
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
            Unstructured = unstructured
            //AdditionalProperties
        };
    }

    private static OBRemittanceInformation2 MapToRemittanceInformation2(
        this PaymentInitiationModelsV3p1p11.RemittanceInformation4 data)
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
            Unstructured = unstructured
            //AdditionalProperties
        };
    }

    private static PaymentInitiationModelsV3p1p11.OBRisk1 MapFromRisk(this OBRisk1 risk)
    {
        return new PaymentInitiationModelsV3p1p11.OBRisk1
        {
            PaymentContextCode = risk.PaymentContextCode switch
            {
                OBRisk1PaymentContextCode.BillingGoodsAndServicesInAdvance => PaymentInitiationModelsV3p1p11
                    .OBRisk1PaymentContextCode.BillingGoodsAndServicesInAdvance,
                OBRisk1PaymentContextCode.BillingGoodsAndServicesInArrears => PaymentInitiationModelsV3p1p11
                    .OBRisk1PaymentContextCode.BillingGoodsAndServicesInArrears,
                OBRisk1PaymentContextCode.EcommerceMerchantInitiatedPayment => PaymentInitiationModelsV3p1p11
                    .OBRisk1PaymentContextCode.EcommerceMerchantInitiatedPayment,
                OBRisk1PaymentContextCode.FaceToFacePointOfSale => PaymentInitiationModelsV3p1p11
                    .OBRisk1PaymentContextCode.FaceToFacePointOfSale,
                OBRisk1PaymentContextCode.TransferToSelf => PaymentInitiationModelsV3p1p11.OBRisk1PaymentContextCode
                    .TransferToSelf,
                OBRisk1PaymentContextCode.TransferToThirdParty => PaymentInitiationModelsV3p1p11
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
                OBInternalExtendedAccountType1Code.Business => PaymentInitiationModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Business,
                OBInternalExtendedAccountType1Code.BusinessSavingsAccount => PaymentInitiationModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.BusinessSavingsAccount,
                OBInternalExtendedAccountType1Code.Charity => PaymentInitiationModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Charity,
                OBInternalExtendedAccountType1Code.Collection => PaymentInitiationModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Collection,
                OBInternalExtendedAccountType1Code.Corporate => PaymentInitiationModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Corporate,
                OBInternalExtendedAccountType1Code.Ewallet => PaymentInitiationModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Ewallet,
                OBInternalExtendedAccountType1Code.Government => PaymentInitiationModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Government,
                OBInternalExtendedAccountType1Code.Investment => PaymentInitiationModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Investment,
                OBInternalExtendedAccountType1Code.ISA => PaymentInitiationModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.ISA,
                OBInternalExtendedAccountType1Code.JointPersonal => PaymentInitiationModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.JointPersonal,
                OBInternalExtendedAccountType1Code.Pension => PaymentInitiationModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Pension,
                OBInternalExtendedAccountType1Code.Personal => PaymentInitiationModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Personal,
                OBInternalExtendedAccountType1Code.PersonalSavingsAccount => PaymentInitiationModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.PersonalSavingsAccount,
                OBInternalExtendedAccountType1Code.Premier => PaymentInitiationModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Premier,
                OBInternalExtendedAccountType1Code.Wealth => PaymentInitiationModelsV3p1p11
                    .OBExternalExtendedAccountType1Code.Wealth,
                null => null,
                _ => throw new ArgumentOutOfRangeException()
            },
            DeliveryAddress = risk.DeliveryAddress?.MapFromDeliveryAddress()
        };
    }

    private static PaymentInitiationModelsV3p1p11.RemittanceInformation2 MapFromRemittanceInformation2(
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

        return new PaymentInitiationModelsV3p1p11.RemittanceInformation2
        {
            Unstructured = unstructured,
            Reference = reference
        };
    }

    private static PaymentInitiationModelsV3p1p11.RemittanceInformation MapFromRemittanceInformation(
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

        return new PaymentInitiationModelsV3p1p11.RemittanceInformation
        {
            Unstructured = unstructured,
            Reference = reference
        };
    }

    private static PaymentInitiationModelsV3p1p11.Data2ReadRefundAccount MapFromReadRefundAccount(
        this Data2ReadRefundAccount data) =>
        data switch
        {
            Data2ReadRefundAccount.No => PaymentInitiationModelsV3p1p11.Data2ReadRefundAccount.No,
            Data2ReadRefundAccount.Yes => PaymentInitiationModelsV3p1p11.Data2ReadRefundAccount.Yes,
            _ => throw new ArgumentOutOfRangeException(nameof(data), data, null)
        };

    private static PaymentInitiationModelsV3p1p11.DebtorAccount2 MapFromDebtorAccount2(
        this DebtorAccount2 data) => new()
    {
        SchemeName = data.SchemeName,
        Identification = data.Identification,
        Name = data.Name,
        SecondaryIdentification = data.SecondaryIdentification
    };

    private static PaymentInitiationModelsV3p1p11.DebtorAccount MapFromDebtorAccount(
        this DebtorAccount data) => new()
    {
        SchemeName = data.SchemeName,
        Identification = data.Identification,
        Name = data.Name,
        SecondaryIdentification = data.SecondaryIdentification
    };

    private static PaymentInitiationModelsV3p1p11.OBPostalAddress6 MapFromCreditorPostalAddress(
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

    private static PaymentInitiationModelsV3p1p11.DeliveryAddress MapFromDeliveryAddress(
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

    private static PaymentInitiationModelsV3p1p11.OBAddressTypeCode MapFromAddressType(
        this OBAddressType2Code addressType) =>
        addressType switch
        {
            OBAddressType2Code.BIZZ => PaymentInitiationModelsV3p1p11.OBAddressTypeCode.Business,
            OBAddressType2Code.DLVY => PaymentInitiationModelsV3p1p11.OBAddressTypeCode.DeliveryTo,
            OBAddressType2Code.MLTO => PaymentInitiationModelsV3p1p11.OBAddressTypeCode.MailTo,
            OBAddressType2Code.PBOX => PaymentInitiationModelsV3p1p11.OBAddressTypeCode.POBox,
            OBAddressType2Code.ADDR => PaymentInitiationModelsV3p1p11.OBAddressTypeCode.Postal,
            OBAddressType2Code.HOME => PaymentInitiationModelsV3p1p11.OBAddressTypeCode.Residential,
            OBAddressType2Code.CORR => PaymentInitiationModelsV3p1p11.OBAddressTypeCode.Correspondence,
            OBAddressType2Code.STAT => PaymentInitiationModelsV3p1p11.OBAddressTypeCode.Statement,
            _ => throw new ArgumentOutOfRangeException(nameof(addressType), addressType, null)
        };

    private static OBAddressType2Code MapToAddressType(
        this PaymentInitiationModelsV3p1p11.OBAddressTypeCode addressType) =>
        addressType switch
        {
            PaymentInitiationModelsV3p1p11.OBAddressTypeCode.Business => OBAddressType2Code.BIZZ,
            PaymentInitiationModelsV3p1p11.OBAddressTypeCode.Correspondence => OBAddressType2Code.CORR,
            PaymentInitiationModelsV3p1p11.OBAddressTypeCode.DeliveryTo => OBAddressType2Code.DLVY,
            PaymentInitiationModelsV3p1p11.OBAddressTypeCode.MailTo => OBAddressType2Code.MLTO,
            PaymentInitiationModelsV3p1p11.OBAddressTypeCode.POBox => OBAddressType2Code.PBOX,
            PaymentInitiationModelsV3p1p11.OBAddressTypeCode.Postal => OBAddressType2Code.ADDR,
            PaymentInitiationModelsV3p1p11.OBAddressTypeCode.Residential => OBAddressType2Code.HOME,
            PaymentInitiationModelsV3p1p11.OBAddressTypeCode.Statement => OBAddressType2Code.STAT,
            _ => throw new ArgumentOutOfRangeException(nameof(addressType), addressType, null)
        };

    private static PaymentInitiationModelsV3p1p11.OBSupplementaryData1 MapFromSupplementaryData(
        this OBSupplementaryData1 data) =>
        new() { AdditionalProperties = data.AdditionalProperties };

    private static PaymentInitiationModelsV3p1p11.Authorisation MapFromAuthorisation(
        this Authorisation data) => new()
    {
        AuthorisationType = data.AuthorisationType switch
        {
            AuthorisationType.Any => PaymentInitiationModelsV3p1p11.AuthorisationType.Any,
            AuthorisationType.Single => PaymentInitiationModelsV3p1p11.AuthorisationType.Single,
            _ => throw new ArgumentOutOfRangeException()
        },
        CompletionDateTime = data.CompletionDateTime
    };

    private static PaymentInitiationModelsV3p1p11.OBSCASupportData1 MapFromSCASupportData(this OBSCASupportData1 data)
        => new()
        {
            RequestedSCAExemptionType = data.RequestedSCAExemptionType switch
            {
                OBSCASupportData1RequestedSCAExemptionType.BillPayment => PaymentInitiationModelsV3p1p11
                    .OBSCASupportData1RequestedSCAExemptionType.BillPayment,
                OBSCASupportData1RequestedSCAExemptionType.ContactlessTravel => PaymentInitiationModelsV3p1p11
                    .OBSCASupportData1RequestedSCAExemptionType.ContactlessTravel,
                OBSCASupportData1RequestedSCAExemptionType.EcommerceGoods => PaymentInitiationModelsV3p1p11
                    .OBSCASupportData1RequestedSCAExemptionType.EcommerceGoods,
                OBSCASupportData1RequestedSCAExemptionType.EcommerceServices => PaymentInitiationModelsV3p1p11
                    .OBSCASupportData1RequestedSCAExemptionType.EcommerceServices,
                OBSCASupportData1RequestedSCAExemptionType.Kiosk => PaymentInitiationModelsV3p1p11
                    .OBSCASupportData1RequestedSCAExemptionType.Kiosk,
                OBSCASupportData1RequestedSCAExemptionType.Parking => PaymentInitiationModelsV3p1p11
                    .OBSCASupportData1RequestedSCAExemptionType.Parking,
                OBSCASupportData1RequestedSCAExemptionType.PartyToParty => PaymentInitiationModelsV3p1p11
                    .OBSCASupportData1RequestedSCAExemptionType.PartyToParty,
                null => null,
                _ => throw new ArgumentOutOfRangeException()
            },
            AppliedAuthenticationApproach = data.AppliedAuthenticationApproach switch
            {
                OBSCASupportData1AppliedAuthenticationApproach.CA => PaymentInitiationModelsV3p1p11
                    .OBSCASupportData1AppliedAuthenticationApproach.CA,
                OBSCASupportData1AppliedAuthenticationApproach.SCA => PaymentInitiationModelsV3p1p11
                    .OBSCASupportData1AppliedAuthenticationApproach.SCA,
                null => null,
                _ => throw new ArgumentOutOfRangeException()
            },
            ReferencePaymentOrderId = data.ReferencePaymentOrderId,
            AdditionalProperties = data.AdditionalProperties
        };
}
