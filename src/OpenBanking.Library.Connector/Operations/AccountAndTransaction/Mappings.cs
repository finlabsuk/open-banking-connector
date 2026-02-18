// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// using AccountAndTransactionModelsV3p1p11 =
//     FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.NSwagAisp.Models;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V4p0.NSwagAisp.Models;

public static class Mappings
{
    public static AccountAndTransactionModelsV3p1p11.OBReadConsent1 MapFromOBReadConsent1(
        OBReadConsent1 objectToSend) =>
        new()
        {
            Data = objectToSend.Data.MapFromData(),
            Risk = new AccountAndTransactionModelsV3p1p11.OBRisk2()
        };

    public static OBReadAccount6 MapToOBReadAccount6(
        AccountAndTransactionModelsV3p1p11.OBReadAccount6 objectReceived) =>
        new()
        {
            Data = objectReceived.Data.MapToData(),
            Links = objectReceived.Links?.MapToLinks(),
            Meta = objectReceived.Meta?.MapToMeta()
        };

    public static OBReadBalance1 MapToOBReadBalance1(
        AccountAndTransactionModelsV3p1p11.OBReadBalance1 objectReceived) =>
        new()
        {
            Data = objectReceived.Data.MapToData2(),
            Links = objectReceived.Links?.MapToLinks(),
            Meta = objectReceived.Meta?.MapToMeta()
        };

    public static OBReadConsentResponse1 MapToOBReadConsentResponse1(
        AccountAndTransactionModelsV3p1p11.OBReadConsentResponse1 objectReceived) =>
        new()
        {
            Data = objectReceived.Data.MapToData5(),
            Risk = new OBRisk2(),
            Links = objectReceived.Links?.MapToLinks(),
            Meta = objectReceived.Meta?.MapToMeta()
        };

    public static OBReadDirectDebit2 MapToOBReadDirectDebit2(
        AccountAndTransactionModelsV3p1p11.OBReadDirectDebit2 objectReceived) =>
        new()
        {
            Data = objectReceived.Data.MapToData6(),
            Links = objectReceived.Links?.MapToLinks(),
            Meta = objectReceived.Meta?.MapToMeta()
        };

    public static OBReadParty2 MapToOBReadParty2(
        AccountAndTransactionModelsV3p1p11.OBReadParty2 objectReceived) =>
        new()
        {
            Data = objectReceived.Data.MapToData8(),
            Links = objectReceived.Links?.MapToLinks(),
            Meta = objectReceived.Meta?.MapToMeta()
        };


    public static OBReadParty3 MapToOBReadParty3(
        AccountAndTransactionModelsV3p1p11.OBReadParty3 objectReceived) =>
        new()
        {
            Data = objectReceived.Data.MapToData9(),
            Links = objectReceived.Links?.MapToLinks(),
            Meta = objectReceived.Meta?.MapToMeta()
        };

    public static OBReadStandingOrder6 MapToOBReadStandingOrder6(
        AccountAndTransactionModelsV3p1p11.OBReadStandingOrder6 objectReceived) =>
        new()
        {
            Data = objectReceived.Data.MapToData12(),
            Links = objectReceived.Links?.MapToLinks(),
            Meta = objectReceived.Meta?.MapToMeta()
        };

    public static OBReadTransaction6 MapToOBReadTransaction6(
        AccountAndTransactionModelsV3p1p11.OBReadTransaction6 objectReceived) =>
        new()
        {
            Data = objectReceived.Data.MapToDataTransaction6(),
            Links = objectReceived.Links?.MapToLinks(),
            Meta = objectReceived.Meta?.MapToMeta()
        };

    private static AccountAndTransactionModelsV3p1p11.Data4 MapFromData(this Data4 data) =>
        new()
        {
            Permissions = data.Permissions
                .Select(p => p.MapFromPermissions())
                .ToList(),
            ExpirationDateTime = data.ExpirationDateTime,
            TransactionFromDateTime = data.TransactionFromDateTime,
            TransactionToDateTime = data.TransactionToDateTime,
            AdditionalProperties = data.AdditionalProperties,
            SupplementaryData = data.SupplementaryData?.MapFromSupplementaryData()
        };

    private static AccountAndTransactionModelsV3p1p11.OBSupplementaryData1 MapFromSupplementaryData(
        this OBSupplementaryData1 data) =>
        new() { AdditionalProperties = data.AdditionalProperties };

    private static Data MapToData(this AccountAndTransactionModelsV3p1p11.Data data) =>
        new()
        {
            Account = data.Account?
                .Select(a => a.MapToAccount6())
                .ToList(),
            AdditionalProperties = data.AdditionalProperties
        };

    private static Data2 MapToData2(this AccountAndTransactionModelsV3p1p11.Data2 data) =>
        new()
        {
            Balance = data.Balance
                .Select(b => b.MapToBalance())
                .ToList(),
            TotalValue = data.TotalValue?.MapToTotalValue(),
            AdditionalProperties = data.AdditionalProperties
        };

    private static TotalValue4 MapToTotalValue(this AccountAndTransactionModelsV3p1p11.TotalValue2 value) =>
        new()
        {
            Amount = value.Amount,
            Currency = value.Currency,
            AdditionalProperties = value.AdditionalProperties
        };

    private static Data5 MapToData5(this AccountAndTransactionModelsV3p1p11.Data5 data) =>
        new()
        {
            ConsentId = data.ConsentId,
            CreationDateTime = data.CreationDateTime,
            Status = data.Status.MapToData5Status(),
            StatusReason = null, // not in v3
            StatusUpdateDateTime = data.StatusUpdateDateTime,
            Permissions =
                data.Permissions
                    .Select(p => p.MapToPermissions2())
                    .ToList(),
            ExpirationDateTime = data.ExpirationDateTime,
            TransactionFromDateTime = data.TransactionFromDateTime,
            TransactionToDateTime = data.TransactionToDateTime,
            AdditionalProperties = data.AdditionalProperties
        };

    private static Data5Status MapToData5Status(this AccountAndTransactionModelsV3p1p11.Data5Status status)
        => status switch
        {
            AccountAndTransactionModelsV3p1p11.Data5Status.Authorised => Data5Status.AUTH,
            AccountAndTransactionModelsV3p1p11.Data5Status.AwaitingAuthorisation => Data5Status.AWAU,
            AccountAndTransactionModelsV3p1p11.Data5Status.Rejected => Data5Status.RJCT,
            AccountAndTransactionModelsV3p1p11.Data5Status.Revoked => Data5Status.EXPD,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };

    private static Permissions2 MapToPermissions2(this AccountAndTransactionModelsV3p1p11.Permissions2 permission)
        => permission switch
        {
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadAccountsBasic => Permissions2.ReadAccountsBasic,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadAccountsDetail => Permissions2.ReadAccountsDetail,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadBalances => Permissions2.ReadBalances,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadBeneficiariesBasic => Permissions2
                .ReadBeneficiariesBasic,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadBeneficiariesDetail => Permissions2
                .ReadBeneficiariesDetail,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadDirectDebits => Permissions2.ReadDirectDebits,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadOffers => Permissions2.ReadOffers,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadPAN => Permissions2.ReadPAN,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadParty => Permissions2.ReadParty,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadPartyPSU => Permissions2.ReadPartyPSU,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadProducts => Permissions2.ReadProducts,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadScheduledPaymentsBasic => Permissions2
                .ReadScheduledPaymentsBasic,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadScheduledPaymentsDetail => Permissions2
                .ReadScheduledPaymentsDetail,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadStandingOrdersBasic => Permissions2
                .ReadStandingOrdersBasic,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadStandingOrdersDetail => Permissions2
                .ReadStandingOrdersDetail,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadStatementsBasic => Permissions2.ReadStatementsBasic,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadStatementsDetail => Permissions2.ReadStatementsDetail,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadTransactionsBasic => Permissions2.ReadTransactionsBasic,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadTransactionsCredits => Permissions2
                .ReadTransactionsCredits,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadTransactionsDebits => Permissions2
                .ReadTransactionsDebits,
            AccountAndTransactionModelsV3p1p11.Permissions2.ReadTransactionsDetail => Permissions2
                .ReadTransactionsDetail,
            _ => throw new ArgumentOutOfRangeException(nameof(permission), permission, null)
        };

    private static AccountAndTransactionModelsV3p1p11.Permissions MapFromPermissions(this Permissions permission)
        => permission switch
        {
            Permissions.ReadAccountsBasic => AccountAndTransactionModelsV3p1p11.Permissions.ReadAccountsBasic,
            Permissions.ReadAccountsDetail => AccountAndTransactionModelsV3p1p11.Permissions.ReadAccountsDetail,
            Permissions.ReadBalances => AccountAndTransactionModelsV3p1p11.Permissions.ReadBalances,
            Permissions.ReadBeneficiariesBasic => AccountAndTransactionModelsV3p1p11.Permissions.ReadBeneficiariesBasic,
            Permissions.ReadBeneficiariesDetail => AccountAndTransactionModelsV3p1p11.Permissions
                .ReadBeneficiariesDetail,
            Permissions.ReadDirectDebits => AccountAndTransactionModelsV3p1p11.Permissions.ReadDirectDebits,
            Permissions.ReadOffers => AccountAndTransactionModelsV3p1p11.Permissions.ReadOffers,
            Permissions.ReadPAN => AccountAndTransactionModelsV3p1p11.Permissions.ReadPAN,
            Permissions.ReadParty => AccountAndTransactionModelsV3p1p11.Permissions.ReadParty,
            Permissions.ReadPartyPSU => AccountAndTransactionModelsV3p1p11.Permissions.ReadPartyPSU,
            Permissions.ReadProducts => AccountAndTransactionModelsV3p1p11.Permissions.ReadProducts,
            Permissions.ReadScheduledPaymentsBasic => AccountAndTransactionModelsV3p1p11.Permissions
                .ReadScheduledPaymentsBasic,
            Permissions.ReadScheduledPaymentsDetail => AccountAndTransactionModelsV3p1p11.Permissions
                .ReadScheduledPaymentsDetail,
            Permissions.ReadStandingOrdersBasic => AccountAndTransactionModelsV3p1p11.Permissions
                .ReadStandingOrdersBasic,
            Permissions.ReadStandingOrdersDetail => AccountAndTransactionModelsV3p1p11.Permissions
                .ReadStandingOrdersDetail,
            Permissions.ReadStatementsBasic => AccountAndTransactionModelsV3p1p11.Permissions.ReadStatementsBasic,
            Permissions.ReadStatementsDetail => AccountAndTransactionModelsV3p1p11.Permissions.ReadStatementsDetail,
            Permissions.ReadTransactionsBasic => AccountAndTransactionModelsV3p1p11.Permissions.ReadTransactionsBasic,
            Permissions.ReadTransactionsCredits => AccountAndTransactionModelsV3p1p11.Permissions
                .ReadTransactionsCredits,
            Permissions.ReadTransactionsDebits => AccountAndTransactionModelsV3p1p11.Permissions.ReadTransactionsDebits,
            Permissions.ReadTransactionsDetail => AccountAndTransactionModelsV3p1p11.Permissions.ReadTransactionsDetail,
            _ => throw new ArgumentOutOfRangeException(nameof(permission), permission, null)
        };


    private static Data6 MapToData6(this AccountAndTransactionModelsV3p1p11.Data6 data) =>
        new()
        {
            DirectDebit = data.DirectDebit?
                .Select(d => d.MapToDirectDebit())
                .ToList(),
            AdditionalProperties = data.AdditionalProperties
        };

    private static Data8 MapToData8(this AccountAndTransactionModelsV3p1p11.Data8 data) =>
        new()
        {
            Party = data.Party?.MapToParty2(),
            AdditionalProperties = data.AdditionalProperties
        };

    private static Data9 MapToData9(this AccountAndTransactionModelsV3p1p11.Data9 data) =>
        new()
        {
            Party = data.Party?
                .Select(p => p.MapToParty2())
                .ToList(),
            AdditionalProperties = data.AdditionalProperties
        };

    private static Data12 MapToData12(this AccountAndTransactionModelsV3p1p11.Data12 data) =>
        new()
        {
            StandingOrder = data.StandingOrder?
                .Select(s => s.MapToStandingOrder())
                .ToList(),
            AdditionalProperties = data.AdditionalProperties
        };

    private static OBReadDataTransaction6 MapToDataTransaction6(
        this AccountAndTransactionModelsV3p1p11.OBReadDataTransaction6 transaction) =>
        new()
        {
            Transaction = transaction.Transaction?
                .Select(t => t.MapToTransaction())
                .ToList()
        };


    private static CreditLine MapToCreditLine(this AccountAndTransactionModelsV3p1p11.CreditLine creditLine) =>
        new()
        {
            Included = creditLine.Included,
            Amount = creditLine.Amount?.MapToAmount5(),
            Type = creditLine.Type?.MapToCreditLineType(),
            AdditionalProperties = creditLine.AdditionalProperties
        };

    private static Amount5 MapToAmount5(this AccountAndTransactionModelsV3p1p11.Amount5 amount) =>
        new()
        {
            Amount = amount.Amount,
            Currency = amount.Currency,
            AdditionalProperties = amount.AdditionalProperties
        };

    private static CreditLineType MapToCreditLineType(this AccountAndTransactionModelsV3p1p11.CreditLineType type) =>
        type switch
        {
            AccountAndTransactionModelsV3p1p11.CreditLineType.Available => CreditLineType.Available,
            AccountAndTransactionModelsV3p1p11.CreditLineType.Credit => CreditLineType.Credit,
            AccountAndTransactionModelsV3p1p11.CreditLineType.Emergency => CreditLineType.Emergency,
            AccountAndTransactionModelsV3p1p11.CreditLineType.PreAgreed => CreditLineType.PreAgreed,
            AccountAndTransactionModelsV3p1p11.CreditLineType.Temporary => CreditLineType.Temporary,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

    private static DirectDebit MapToDirectDebit(this AccountAndTransactionModelsV3p1p11.DirectDebit directDebit) =>
        new()
        {
            AccountId = directDebit.AccountId,
            DirectDebitId = directDebit.DirectDebitId,
            DirectDebitStatusCode =
                directDebit.DirectDebitStatusCode?.MapToDirectDebitStatusCode(),
            MandateRelatedInformation = directDebit.MapToMandateRelatedInformation(),
            Name = directDebit.Name,
            PreviousPaymentDateTime = directDebit.PreviousPaymentDateTime,
            PreviousPaymentAmount = directDebit.PreviousPaymentAmount?.MapToPreviousPaymentAmount(),
            AdditionalProperties = directDebit.AdditionalProperties
        };

    private static ExternalMandateStatus1Code MapToDirectDebitStatusCode(
        this AccountAndTransactionModelsV3p1p11.OBExternalDirectDebitStatus1Code status) =>
        status switch
        {
            AccountAndTransactionModelsV3p1p11.OBExternalDirectDebitStatus1Code.Active => ExternalMandateStatus1Code
                .ACTV,
            AccountAndTransactionModelsV3p1p11.OBExternalDirectDebitStatus1Code.Inactive => ExternalMandateStatus1Code
                .SUSP, // decision: use SUSP ("put on hold")
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };

    private static OBMandateRelatedInformation1 MapToMandateRelatedInformation(
        this AccountAndTransactionModelsV3p1p11.DirectDebit directDebit) =>
        new()
        {
            MandateIdentification = directDebit.MandateIdentification,
            Classification = null, // not in v3
            CategoryPurposeCode = null, // not in v3
            FirstPaymentDateTime = null, // not in v3
            RecurringPaymentDateTime = null, // not in v3
            FinalPaymentDateTime = null, // not in v3
            Frequency =
                directDebit.Frequency?.MapToFrequency6()
                ?? new OBFrequency6
                {
                    Type = OBFrequency6Code.NONE.ToString(),
                    CountPerPeriod = null,
                    PointInTime = null // not in v3
                    //AdditionalProperties
                },
            Reason = null // not in v3
            //AdditionalProperties
        };

    private static OBFrequency6 MapToFrequency6(
        this string frequency) =>
        new()
        {
            // decision: use mappings to Type shown and CountPerPeriod = 1
            Type = (frequency switch
            {
                "UK.OBIE.Monthly" => OBFrequency6Code.MNTH,
                "UK.OBIE.Quarterly" => OBFrequency6Code.QURT,
                "UK.OBIE.HalfYearly" => OBFrequency6Code.MIAN,
                "UK.OBIE.Weekly" => OBFrequency6Code.WEEK,
                "UK.OBIE.Annual" => OBFrequency6Code.YEAR,
                "UK.OBIE.Fortnightly" => OBFrequency6Code.FRTN,
                "UK.OBIE.Daily" => OBFrequency6Code.DAIL,
                "UK.OBIE.NotKnown" => OBFrequency6Code.NONE,
                "UK.NWB.BiMonthly" => OBFrequency6Code.TWMH,
                "UK.NWB.FourMonthly" => OBFrequency6Code.FOMH,
                "UK.RBS.BiMonthly" => OBFrequency6Code.TWMH,
                "UK.RBS.FourMonthly" => OBFrequency6Code.FOMH,
                "UK.UBN.BiMonthly" => OBFrequency6Code.TWMH,
                "UK.UBN.FourMonthly" => OBFrequency6Code.FOMH,
                _ => throw new ArgumentOutOfRangeException(nameof(frequency), frequency, null)
            }).ToString(),
            CountPerPeriod = 1,
            PointInTime = null // not in v3
            //AdditionalProperties
        };

    private static OBActiveOrHistoricCurrencyAndAmount_0 MapToPreviousPaymentAmount(
        this AccountAndTransactionModelsV3p1p11.OBActiveOrHistoricCurrencyAndAmount_0 amount) =>
        new()
        {
            Amount = amount.Amount,
            Currency = amount.Currency,
            AdditionalProperties = amount.AdditionalProperties
        };

    private static OBPostalAddress7 MapToPostalAddress(this AccountAndTransactionModelsV3p1p11.Address address) =>
        new()
        {
            AddressType = address.AddressType?.MapToAddressType(),
            Department = null, // not in v3
            SubDepartment = null, // not in v3
            AddressLine = address.AddressLine,
            StreetName = address.StreetName,
            BuildingNumber = address.BuildingNumber,
            BuildingName = null, // not in v3
            Floor = null, // not in v3
            UnitNumber = null, // not in v3
            Room = null, // not in v3
            PostBox = null, // not in v3
            TownLocationName = null, // not in v3
            DistrictName = null, // not in v3
            CareOf = null, // not in v3
            PostCode = address.PostCode,
            TownName = address.TownName,
            CountrySubDivision = address.CountrySubDivision,
            Country = address.Country,
            AdditionalProperties = address.AdditionalProperties
        };

    private static OBAddressType2Code MapToAddressType(
        this AccountAndTransactionModelsV3p1p11.OBAddressTypeCode addressType) =>
        addressType switch
        {
            AccountAndTransactionModelsV3p1p11.OBAddressTypeCode.Business => OBAddressType2Code.BIZZ,
            AccountAndTransactionModelsV3p1p11.OBAddressTypeCode.Correspondence => OBAddressType2Code.CORR,
            AccountAndTransactionModelsV3p1p11.OBAddressTypeCode.DeliveryTo => OBAddressType2Code.DLVY,
            AccountAndTransactionModelsV3p1p11.OBAddressTypeCode.MailTo => OBAddressType2Code.MLTO,
            AccountAndTransactionModelsV3p1p11.OBAddressTypeCode.POBox => OBAddressType2Code.PBOX,
            AccountAndTransactionModelsV3p1p11.OBAddressTypeCode.Postal => OBAddressType2Code.ADDR,
            AccountAndTransactionModelsV3p1p11.OBAddressTypeCode.Residential => OBAddressType2Code.HOME,
            AccountAndTransactionModelsV3p1p11.OBAddressTypeCode.Statement => OBAddressType2Code.STAT,
            _ => throw new ArgumentOutOfRangeException(nameof(addressType), addressType, null)
        };

    private static OBActiveOrHistoricCurrencyAndAmount_2 MapToFirstPaymentAmount(
        this AccountAndTransactionModelsV3p1p11.OBActiveOrHistoricCurrencyAndAmount_2 amount) =>
        new()
        {
            Amount = amount.Amount,
            Currency = amount.Currency,
            AdditionalProperties = amount.AdditionalProperties
        };

    private static OBActiveOrHistoricCurrencyAndAmount_3 MapToNextPaymentAmount(
        this AccountAndTransactionModelsV3p1p11.OBActiveOrHistoricCurrencyAndAmount_3 amount) =>
        new()
        {
            Amount = amount.Amount,
            Currency = amount.Currency,
            AdditionalProperties = amount.AdditionalProperties
        };

    private static OBActiveOrHistoricCurrencyAndAmount_11 MapToLastPaymentAmount(
        this AccountAndTransactionModelsV3p1p11.OBActiveOrHistoricCurrencyAndAmount_11 amount) =>
        new()
        {
            Amount = amount.Amount,
            Currency = amount.Currency,
            AdditionalProperties = amount.AdditionalProperties
        };

    private static OBActiveOrHistoricCurrencyAndAmount_4 MapToFinalPaymentAmount(
        this AccountAndTransactionModelsV3p1p11.OBActiveOrHistoricCurrencyAndAmount_4 amount) =>
        new()
        {
            Amount = amount.Amount,
            Currency = amount.Currency,
            AdditionalProperties = amount.AdditionalProperties
        };

    private static OBBranchAndFinancialInstitutionIdentification5_1 MapToCreditorAgent(
        this AccountAndTransactionModelsV3p1p11.OBBranchAndFinancialInstitutionIdentification5_1 agent) =>
        new()
        {
            SchemeName = agent.SchemeName,
            Identification = agent.Identification,
            Name = null, // not in v3
            PostalAddress = null, // not in v3
            LEI = null, // not in v3
            AdditionalProperties = agent.AdditionalProperties
        };

    private static OBCashAccount5_1 MapToCreditorAccount(
        this AccountAndTransactionModelsV3p1p11.OBCashAccount5_1 account) =>
        new()
        {
            SchemeName = account.SchemeName,
            Identification = account.Identification,
            Name = account.Name,
            SecondaryIdentification = account.SecondaryIdentification,
            Proxy = null, // not in v3
            AdditionalProperties = account.AdditionalProperties
        };

    private static OBSupplementaryData1? MapToSupplementaryData(
        AccountAndTransactionModelsV3p1p11.OBSupplementaryData1? data,
        KeyValuePair<string, object>? extraProperty1 = null,
        KeyValuePair<string, object>? extraProperty2 = null)
    {
        if (data is null &&
            !extraProperty1.HasValue &&
            !extraProperty2.HasValue)
        {
            return null;
        }

        IDictionary<string, object> additionalProperties =
            data?.AdditionalProperties ?? new Dictionary<string, object>();
        if (extraProperty1.HasValue)
        {
            additionalProperties.Add(extraProperty1.Value);
        }
        if (extraProperty2.HasValue)
        {
            additionalProperties.Add(extraProperty2.Value);
        }
        var supplementaryData = new OBSupplementaryData1 { AdditionalProperties = additionalProperties };
        return supplementaryData;
    }

    private static OBTransaction6 MapToTransaction(
        this AccountAndTransactionModelsV3p1p11.OBTransaction6 transaction) =>
        new()
        {
            AccountId = transaction.AccountId,
            TransactionId = transaction.TransactionId,
            TransactionReference = transaction.TransactionReference,
            StatementReference = transaction.StatementReference,
            CreditDebitIndicator = transaction.CreditDebitIndicator.MapToCreditDebitIndicator(),
            Status = transaction.Status.MapToEntryStatus(),
            TransactionMutability = transaction.TransactionMutability?.MapToTransactionMutability(),
            BookingDateTime = transaction.BookingDateTime,
            ValueDateTime = transaction.ValueDateTime,
            TransactionInformation = transaction.TransactionInformation,
            AddressLine = transaction.AddressLine,
            Amount = transaction.Amount.MapToAmount9(),
            ChargeAmount = transaction.ChargeAmount?.MapToChargeAmount(),
            CurrencyExchange = transaction.CurrencyExchange?.MapToCurrencyExchange(),
            BankTransactionCode = null, // v3 object passed in SupplementaryData
            ProprietaryBankTransactionCode =
                transaction.ProprietaryBankTransactionCode?.MapToProprietaryBankTransactionCode(),
            ExtendedProprietaryBankTransactionCodes = null, // not in v3
            CardInstrument = transaction.CardInstrument?.MapToCardInstrument(),
            SupplementaryData = MapToSupplementaryData(
                transaction.SupplementaryData,
                transaction.BankTransactionCode?.Code is not null
                    ? new KeyValuePair<string, object>(
                        "V3BankTransactionCodeCode",
                        transaction.BankTransactionCode?.Code!)
                    : null,
                transaction.BankTransactionCode?.SubCode is not null
                    ? new KeyValuePair<string, object>(
                        "V3BankTransactionCodeSubCode",
                        transaction.BankTransactionCode?.SubCode!)
                    : null),
            CategoryPurposeCode = null, // not in v3
            PaymentPurposeCode = null, // not in v3
            UltimateCreditor = null, // not in v3
            UltimateDebtor = null, // not in v3
            Balance = transaction.Balance?.MapToTransactionCashBalance(),
            MerchantDetails = transaction.MerchantDetails?.MapToMerchantDetails(),
            CreditorAgent = transaction.CreditorAgent?.MapToCreditorAgent6(),
            CreditorAccount = transaction.CreditorAccount?.MapToCreditorAccount6(),
            DebtorAgent = transaction.DebtorAgent?.MapToDebtorAgent(),
            DebtorAccount = transaction.DebtorAccount?.MapToDebtorAccount()
        };

    private static Balance MapToBalance(this AccountAndTransactionModelsV3p1p11.Balance balance) =>
        new()
        {
            AccountId = balance.AccountId,
            Amount = balance.Amount.MapToAmount3(),
            CreditDebitIndicator = balance.CreditDebitIndicator.MapToCreditDebitIndicator2(),
            Type = balance.Type.MapToBalanceType(),
            V3Type = balance.Type, // decision: map Type to V3Type to avoid lossy mapping
            DateTime = balance.DateTime,
            CreditLine = balance.CreditLine?
                .Select(c => c.MapToCreditLine())
                .ToList(),
            LocalAmount = balance.LocalAmount?.MapToLocalAmount3(),
            AdditionalProperties = balance.AdditionalProperties
        };

    private static Amount3 MapToAmount3(this AccountAndTransactionModelsV3p1p11.Amount3 amount) =>
        new()
        {
            Amount = amount.Amount,
            Currency = amount.Currency,
            SubType = amount.SubType?.MapToAmount3SubType(),
            AdditionalProperties = amount.AdditionalProperties
        };

    private static LocalAmount3 MapToLocalAmount3(this AccountAndTransactionModelsV3p1p11.LocalAmount2 amount) =>
        new()
        {
            Amount = amount.Amount,
            Currency = amount.Currency,
            SubType = amount.SubType?.MapToLocalAmount3SubType(),
            AdditionalProperties = amount.AdditionalProperties
        };

    private static Amount3SubType MapToAmount3SubType(this AccountAndTransactionModelsV3p1p11.Amount3SubType amount) =>
        amount switch
        {
            AccountAndTransactionModelsV3p1p11.Amount3SubType.BaseCurrency => Amount3SubType.BCUR,
            AccountAndTransactionModelsV3p1p11.Amount3SubType.LocalCurrency => Amount3SubType.LCUR,
            _ => throw new ArgumentOutOfRangeException(nameof(amount), amount, null)
        };

    private static LocalAmount3SubType MapToLocalAmount3SubType(
        this AccountAndTransactionModelsV3p1p11.LocalAmount2SubType amount) =>
        amount switch
        {
            AccountAndTransactionModelsV3p1p11.LocalAmount2SubType.BaseCurrency => LocalAmount3SubType.BCUR,
            AccountAndTransactionModelsV3p1p11.LocalAmount2SubType.LocalCurrency => LocalAmount3SubType.LCUR,
            _ => throw new ArgumentOutOfRangeException(nameof(amount), amount, null)
        };

    private static OBCreditDebitCode_1 MapToCreditDebitIndicator(
        this AccountAndTransactionModelsV3p1p11.OBCreditDebitCode_1 indicator) =>
        indicator switch
        {
            AccountAndTransactionModelsV3p1p11.OBCreditDebitCode_1.Credit => OBCreditDebitCode_1.Credit,
            AccountAndTransactionModelsV3p1p11.OBCreditDebitCode_1.Debit => OBCreditDebitCode_1.Debit,
            _ => throw new ArgumentOutOfRangeException(nameof(indicator), indicator, null)
        };

    private static ExternalEntryStatus1Code MapToEntryStatus(
        this AccountAndTransactionModelsV3p1p11.OBEntryStatus1Code status) =>
        status switch
        {
            AccountAndTransactionModelsV3p1p11.OBEntryStatus1Code.Booked => ExternalEntryStatus1Code.BOOK,
            AccountAndTransactionModelsV3p1p11.OBEntryStatus1Code.Pending => ExternalEntryStatus1Code.PDNG,
            AccountAndTransactionModelsV3p1p11.OBEntryStatus1Code.Rejected => ExternalEntryStatus1Code.RJCT,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };

    private static OBInternalTransactionMutability1Code MapToTransactionMutability(
        this AccountAndTransactionModelsV3p1p11.OBTransactionMutability1Code mutability) =>
        mutability switch
        {
            AccountAndTransactionModelsV3p1p11.OBTransactionMutability1Code.Mutable =>
                OBInternalTransactionMutability1Code.Mutable,
            AccountAndTransactionModelsV3p1p11.OBTransactionMutability1Code.Immutable =>
                OBInternalTransactionMutability1Code.Immutable,
            _ => throw new ArgumentOutOfRangeException(nameof(mutability), mutability, null)
        };

    private static OBActiveOrHistoricCurrencyAndAmount_9 MapToAmount9(
        this AccountAndTransactionModelsV3p1p11.OBActiveOrHistoricCurrencyAndAmount_9 amount) =>
        new()
        {
            Amount = amount.Amount,
            Currency = amount.Currency,
            AdditionalProperties = amount.AdditionalProperties
        };

    private static OBActiveOrHistoricCurrencyAndAmount_10 MapToChargeAmount(
        this AccountAndTransactionModelsV3p1p11.OBActiveOrHistoricCurrencyAndAmount_10 amount) =>
        new()
        {
            Amount = amount.Amount,
            Currency = amount.Currency,
            AdditionalProperties = amount.AdditionalProperties
        };

    private static OBCurrencyExchange5 MapToCurrencyExchange(
        this AccountAndTransactionModelsV3p1p11.OBCurrencyExchange5 exchange) =>
        new()
        {
            SourceCurrency = exchange.SourceCurrency,
            TargetCurrency = exchange.TargetCurrency,
            UnitCurrency = exchange.UnitCurrency,
            ExchangeRate = exchange.ExchangeRate,
            ContractIdentification = exchange.ContractIdentification,
            QuotationDate = exchange.QuotationDate,
            InstructedAmount = exchange.InstructedAmount?.MapToInstructedAmount(),
            AdditionalProperties = exchange.AdditionalProperties
        };

    private static InstructedAmount MapToInstructedAmount(
        this AccountAndTransactionModelsV3p1p11.InstructedAmount amount) =>
        new()
        {
            Amount = amount.Amount,
            Currency = amount.Currency,
            AdditionalProperties = amount.AdditionalProperties
        };

    private static ProprietaryBankTransactionCodeStructure1 MapToProprietaryBankTransactionCode(
        this AccountAndTransactionModelsV3p1p11.ProprietaryBankTransactionCodeStructure1 transactionCode) =>
        new()
        {
            Code = transactionCode.Code,
            Issuer = transactionCode.Issuer
        };

    private static OBTransactionCardInstrument1 MapToCardInstrument(
        this AccountAndTransactionModelsV3p1p11.OBTransactionCardInstrument1 instrument) =>
        new()
        {
            CardSchemeName =
                instrument.CardSchemeName?.MapToCardSchemeName(),
            AuthorisationType =
                instrument.AuthorisationType?.MapToAuthorisationType(),
            Name = instrument.Name,
            Identification = instrument.Identification
        };

    private static OBTransactionCashBalance MapToTransactionCashBalance(
        this AccountAndTransactionModelsV3p1p11.OBTransactionCashBalance balance) =>
        new()
        {
            Amount = balance.Amount?.MapToAmount8(),
            CreditDebitIndicator = balance.CreditDebitIndicator.MapToCreditDebitIndicator2(),
            Type = balance.Type.MapToBalanceType(), // decision: return extra property to avoid lossy mapping
            V3IsClearedBalanceType =
                balance.Type is AccountAndTransactionModelsV3p1p11.OBBalanceType1Code.ClosingCleared
                    or AccountAndTransactionModelsV3p1p11.OBBalanceType1Code.InterimCleared
                    or AccountAndTransactionModelsV3p1p11.OBBalanceType1Code.OpeningCleared
        };

    private static Amount MapToAmount8(
        this AccountAndTransactionModelsV3p1p11.Amount amount) =>
        new()
        {
            Amount1 = amount.Amount1,
            Currency = amount.Currency,
            AdditionalProperties = amount.AdditionalProperties
        };

    private static OBMerchantDetails1 MapToMerchantDetails(
        this AccountAndTransactionModelsV3p1p11.OBMerchantDetails1 details) =>
        new()
        {
            MerchantName = details.MerchantName,
            MerchantCategoryCode = details.MerchantCategoryCode,
            AdditionalProperties = details.AdditionalProperties
        };

    private static OBBranchAndFinancialInstitutionIdentification6_1 MapToCreditorAgent6(
        this AccountAndTransactionModelsV3p1p11.OBBranchAndFinancialInstitutionIdentification6_1 agent) =>
        new()
        {
            SchemeName = agent.SchemeName,
            Identification = agent.Identification,
            Name = agent.Name,
            LEI = null, // not in v3
            PostalAddress = agent.PostalAddress?.MapToPostalAddress(),
            AdditionalProperties = agent.AdditionalProperties
        };

    private static OBCashAccount6_0 MapToCreditorAccount6(
        this AccountAndTransactionModelsV3p1p11.OBCashAccount6_0 account) =>
        new()
        {
            SchemeName = account.SchemeName,
            Identification = account.Identification,
            Name = account.Name,
            SecondaryIdentification = account.SecondaryIdentification,
            Proxy = null, // not in v3
            AdditionalProperties = account.AdditionalProperties
        };

    private static OBBranchAndFinancialInstitutionIdentification6_2 MapToDebtorAgent(
        this AccountAndTransactionModelsV3p1p11.OBBranchAndFinancialInstitutionIdentification6_2 agent) =>
        new()
        {
            SchemeName = agent.SchemeName,
            Identification = agent.Identification,
            Name = agent.Name,
            LEI = null, // not in v3
            PostalAddress = agent.PostalAddress?.MapToPostalAddress(),
            AdditionalProperties = agent.AdditionalProperties
        };

    private static OBCashAccount6_1 MapToDebtorAccount(
        this AccountAndTransactionModelsV3p1p11.OBCashAccount6_1 account) =>
        new()
        {
            SchemeName = account.SchemeName,
            Identification = account.Identification,
            Name = account.Name,
            SecondaryIdentification = account.SecondaryIdentification,
            Proxy = null, // not in v3
            AdditionalProperties = account.AdditionalProperties
        };

    private static OBAccount6 MapToAccount6(this AccountAndTransactionModelsV3p1p11.OBAccount6 account) =>
        new()
        {
            AccountId = account.AccountId,
            Status = account.Status?.MapToStatus(),
            StatusUpdateDateTime = account.StatusUpdateDateTime,
            Currency = account.Currency,
            AccountCategory = account.AccountType?.MapToAccountCategory(),
            AccountTypeCode = null, // decision use V3AccountSubType as otherwise mapping is lossy
            V3AccountSubType = account.AccountSubType,
            Description = account.Description,
            Nickname = account.Nickname,
            OpeningDate = account.OpeningDate,
            MaturityDate = account.MaturityDate,
            SwitchStatus = account.SwitchStatus,
            Account = account.Account?
                .Select(a => a.MapToAccount())
                .ToList(),
            StatementFrequencyAndFormat = null, // not in v3
            Servicer = account.Servicer?.MapToServicer()
        };

    private static OBParty2 MapToParty2(this AccountAndTransactionModelsV3p1p11.OBParty2 party) =>
        new()
        {
            PartyId = party.PartyId,
            PartyNumber = party.PartyNumber,
            PartyType = party.PartyType?.MapToPartyType(),
            Name = party.Name,
            FullLegalName = party.FullLegalName,
            LegalStructure = party.LegalStructure,
            LEI = null, // not in v3
            BeneficialOwnership = party.BeneficialOwnership,
            AccountRole = party.AccountRole,
            EmailAddress = party.EmailAddress,
            Phone = party.Phone,
            Mobile = party.Mobile,
            Relationships = party.Relationships?.MapToRelationships(),
            Address = party.Address?
                .Select(a => a.MapToPostalAddress())
                .ToList()
        };

    private static OBPartyRelationships1 MapToRelationships(
        this AccountAndTransactionModelsV3p1p11.OBPartyRelationships1 relationships) =>
        new()
        {
            Account = relationships.Account?.MapToAccount3(),
            AdditionalProperties = relationships.AdditionalProperties
        };

    private static Account3 MapToAccount3(this AccountAndTransactionModelsV3p1p11.Account3 account) =>
        new()
        {
            Related = account.Related,
            Id = account.Id,
            AdditionalProperties = account.AdditionalProperties
        };

    private static Links MapToLinks(this AccountAndTransactionModelsV3p1p11.Links links) =>
        new()
        {
            Self = links.Self,
            First = links.First,
            Prev = links.Prev,
            Next = links.Next,
            Last = links.Last
        };

    private static Meta MapToMeta(this AccountAndTransactionModelsV3p1p11.Meta meta) =>
        new()
        {
            TotalPages = meta.TotalPages,
            FirstAvailableDateTime = meta.FirstAvailableDateTime,
            LastAvailableDateTime = meta.LastAvailableDateTime
        };

    private static Account MapToAccount(this AccountAndTransactionModelsV3p1p11.Account account) =>
        new()
        {
            SchemeName = account.SchemeName,
            Identification = account.Identification,
            Name = account.Name,
            LEI = null, // not in v3
            SecondaryIdentification = account.SecondaryIdentification,
            AdditionalProperties = account.AdditionalProperties
        };

    private static OBBranchAndFinancialInstitutionIdentification5_0
        MapToServicer(
            this AccountAndTransactionModelsV3p1p11.OBBranchAndFinancialInstitutionIdentification5_0 servicer) =>
        new()
        {
            SchemeName = servicer.SchemeName,
            Identification = servicer.Identification,
            Name = null, // not in v3
            AdditionalProperties = servicer.AdditionalProperties
        };

    private static OBInternalAccountType1Code MapToAccountCategory(
        this AccountAndTransactionModelsV3p1p11.OBExternalAccountType1Code accountType) =>
        accountType switch
        {
            AccountAndTransactionModelsV3p1p11.OBExternalAccountType1Code.Business => OBInternalAccountType1Code
                .Business,
            AccountAndTransactionModelsV3p1p11.OBExternalAccountType1Code.Personal => OBInternalAccountType1Code
                .Personal,
            _ => throw new ArgumentOutOfRangeException(nameof(accountType), accountType, null)
        };

    private static OBInternalAccountStatus1Code MapToStatus(
        this AccountAndTransactionModelsV3p1p11.OBAccountStatus1Code status) =>
        status switch
        {
            AccountAndTransactionModelsV3p1p11.OBAccountStatus1Code.Deleted => OBInternalAccountStatus1Code.Deleted,
            AccountAndTransactionModelsV3p1p11.OBAccountStatus1Code.Disabled => OBInternalAccountStatus1Code.Disabled,
            AccountAndTransactionModelsV3p1p11.OBAccountStatus1Code.Enabled => OBInternalAccountStatus1Code.Enabled,
            AccountAndTransactionModelsV3p1p11.OBAccountStatus1Code.Pending => OBInternalAccountStatus1Code.Pending,
            AccountAndTransactionModelsV3p1p11.OBAccountStatus1Code.ProForma => OBInternalAccountStatus1Code.ProForma,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };

    private static OBTransactionCardInstrument1CardSchemeName MapToCardSchemeName(
        this AccountAndTransactionModelsV3p1p11.OBTransactionCardInstrument1CardSchemeName schemeName) =>
        schemeName switch
        {
            AccountAndTransactionModelsV3p1p11.OBTransactionCardInstrument1CardSchemeName.AmericanExpress =>
                OBTransactionCardInstrument1CardSchemeName.AmericanExpress,
            AccountAndTransactionModelsV3p1p11.OBTransactionCardInstrument1CardSchemeName.Diners =>
                OBTransactionCardInstrument1CardSchemeName.Diners,
            AccountAndTransactionModelsV3p1p11.OBTransactionCardInstrument1CardSchemeName.Discover =>
                OBTransactionCardInstrument1CardSchemeName.Discover,
            AccountAndTransactionModelsV3p1p11.OBTransactionCardInstrument1CardSchemeName.MasterCard =>
                OBTransactionCardInstrument1CardSchemeName.MasterCard,
            AccountAndTransactionModelsV3p1p11.OBTransactionCardInstrument1CardSchemeName.VISA =>
                OBTransactionCardInstrument1CardSchemeName.VISA,
            _ => throw new ArgumentOutOfRangeException(nameof(schemeName), schemeName, null)
        };

    private static OBTransactionCardInstrument1AuthorisationType MapToAuthorisationType(
        this AccountAndTransactionModelsV3p1p11.OBTransactionCardInstrument1AuthorisationType authType) =>
        authType switch
        {
            AccountAndTransactionModelsV3p1p11.OBTransactionCardInstrument1AuthorisationType.ConsumerDevice =>
                OBTransactionCardInstrument1AuthorisationType.ConsumerDevice,
            AccountAndTransactionModelsV3p1p11.OBTransactionCardInstrument1AuthorisationType.Contactless =>
                OBTransactionCardInstrument1AuthorisationType.Contactless,
            AccountAndTransactionModelsV3p1p11.OBTransactionCardInstrument1AuthorisationType.None =>
                OBTransactionCardInstrument1AuthorisationType.None,
            AccountAndTransactionModelsV3p1p11.OBTransactionCardInstrument1AuthorisationType.PIN =>
                OBTransactionCardInstrument1AuthorisationType.PIN,
            _ => throw new ArgumentOutOfRangeException(nameof(authType), authType, null)
        };

    private static OBCreditDebitCode_2 MapToCreditDebitIndicator2(
        this AccountAndTransactionModelsV3p1p11.OBCreditDebitCode_2 indicator) =>
        indicator switch
        {
            AccountAndTransactionModelsV3p1p11.OBCreditDebitCode_2.Credit => OBCreditDebitCode_2.Credit,
            AccountAndTransactionModelsV3p1p11.OBCreditDebitCode_2.Debit => OBCreditDebitCode_2.Debit,
            _ => throw new ArgumentOutOfRangeException(nameof(indicator), indicator, null)
        };

    private static OBBalanceType1CodeV4 MapToBalanceType(
        this AccountAndTransactionModelsV3p1p11.OBBalanceType1Code type) =>
        type switch
        {
            AccountAndTransactionModelsV3p1p11.OBBalanceType1Code.ClosingAvailable => OBBalanceType1CodeV4.CLAV,
            AccountAndTransactionModelsV3p1p11.OBBalanceType1Code.ClosingBooked => OBBalanceType1CodeV4.CLBD,
            AccountAndTransactionModelsV3p1p11.OBBalanceType1Code
                .ClosingCleared => OBBalanceType1CodeV4.CLAV, // decision: use available for cleared
            AccountAndTransactionModelsV3p1p11.OBBalanceType1Code.Expected => OBBalanceType1CodeV4.XPCD,
            AccountAndTransactionModelsV3p1p11.OBBalanceType1Code.ForwardAvailable => OBBalanceType1CodeV4.FWAV,
            AccountAndTransactionModelsV3p1p11.OBBalanceType1Code.Information => OBBalanceType1CodeV4.INFO,
            AccountAndTransactionModelsV3p1p11.OBBalanceType1Code.InterimAvailable => OBBalanceType1CodeV4.ITAV,
            AccountAndTransactionModelsV3p1p11.OBBalanceType1Code.InterimBooked => OBBalanceType1CodeV4.ITBD,
            AccountAndTransactionModelsV3p1p11.OBBalanceType1Code
                .InterimCleared => OBBalanceType1CodeV4.ITAV, // decision: use available for cleared
            AccountAndTransactionModelsV3p1p11.OBBalanceType1Code.OpeningAvailable => OBBalanceType1CodeV4.OPAV,
            AccountAndTransactionModelsV3p1p11.OBBalanceType1Code.OpeningBooked => OBBalanceType1CodeV4.OPBD,
            AccountAndTransactionModelsV3p1p11.OBBalanceType1Code
                .OpeningCleared => OBBalanceType1CodeV4.OPAV, // decision: use available for cleared
            AccountAndTransactionModelsV3p1p11.OBBalanceType1Code
                .PreviouslyClosedBooked => OBBalanceType1CodeV4.PRCD,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

    private static OBPostalAddress7 MapToPostalAddress(
        this AccountAndTransactionModelsV3p1p11.OBPostalAddress6 address) =>
        new()
        {
            AddressType = address.AddressType?.MapToAddressType2(),
            Department = address.Department,
            SubDepartment = address.SubDepartment,
            AddressLine = address.AddressLine,
            StreetName = address.StreetName,
            BuildingNumber = address.BuildingNumber,
            BuildingName = null, // not in v3
            Floor = null, // not in v3
            UnitNumber = null, // not in v3
            Room = null, // not in v3
            PostBox = null, // not in v3
            TownLocationName = null, // not in v3
            DistrictName = null, // not in v3
            CareOf = null, // not in v3
            PostCode = address.PostCode,
            TownName = address.TownName,
            CountrySubDivision = address.CountrySubDivision,
            Country = address.Country,
            AdditionalProperties = address.AdditionalProperties
        };

    private static OBAddressType2Code MapToAddressType2(
        this AccountAndTransactionModelsV3p1p11.OBAddressTypeCode addressType) =>
        addressType switch
        {
            AccountAndTransactionModelsV3p1p11.OBAddressTypeCode.Business => OBAddressType2Code.BIZZ,
            AccountAndTransactionModelsV3p1p11.OBAddressTypeCode.Correspondence => OBAddressType2Code.CORR,
            AccountAndTransactionModelsV3p1p11.OBAddressTypeCode.DeliveryTo => OBAddressType2Code.DLVY,
            AccountAndTransactionModelsV3p1p11.OBAddressTypeCode.MailTo => OBAddressType2Code.MLTO,
            AccountAndTransactionModelsV3p1p11.OBAddressTypeCode.POBox => OBAddressType2Code.PBOX,
            AccountAndTransactionModelsV3p1p11.OBAddressTypeCode.Postal => OBAddressType2Code.ADDR,
            AccountAndTransactionModelsV3p1p11.OBAddressTypeCode.Residential => OBAddressType2Code.HOME,
            AccountAndTransactionModelsV3p1p11.OBAddressTypeCode.Statement => OBAddressType2Code.STAT,
            _ => throw new ArgumentOutOfRangeException(nameof(addressType), addressType, null)
        };

    private static OBInternalPartyType1Code MapToPartyType(
        this AccountAndTransactionModelsV3p1p11.OBExternalPartyType1Code partyType) =>
        partyType switch
        {
            AccountAndTransactionModelsV3p1p11.OBExternalPartyType1Code.Delegate => OBInternalPartyType1Code.Delegate,
            AccountAndTransactionModelsV3p1p11.OBExternalPartyType1Code.Joint => OBInternalPartyType1Code.Joint,
            AccountAndTransactionModelsV3p1p11.OBExternalPartyType1Code.Sole => OBInternalPartyType1Code.Sole,
            _ => throw new ArgumentOutOfRangeException(nameof(partyType), partyType, null)
        };

    private static OBStandingOrder6 MapToStandingOrder(
        this AccountAndTransactionModelsV3p1p11.OBStandingOrder6 standingOrder) =>
        new()
        {
            AccountId = standingOrder.AccountId,
            StandingOrderId = standingOrder.StandingOrderId,
            NextPaymentDateTime = standingOrder.NextPaymentDateTime,
            LastPaymentDateTime = standingOrder.LastPaymentDateTime,
            NumberOfPayments = standingOrder.NumberOfPayments,
            StandingOrderStatusCode =
                standingOrder.StandingOrderStatusCode?.MapToStandingOrderStatusCode(),
            FirstPaymentAmount = standingOrder.FirstPaymentAmount?.MapToFirstPaymentAmount(),
            NextPaymentAmount = standingOrder.NextPaymentAmount?.MapToNextPaymentAmount(),
            LastPaymentAmount = standingOrder.LastPaymentAmount?.MapToLastPaymentAmount(),
            FinalPaymentAmount = standingOrder.FinalPaymentAmount?.MapToFinalPaymentAmount(),
            CreditorAgent = standingOrder.CreditorAgent?.MapToCreditorAgent(),
            CreditorAccount = standingOrder.CreditorAccount?.MapToCreditorAccount(),
            SupplementaryData = MapToSupplementaryData(standingOrder.SupplementaryData),
            MandateRelatedInformation = standingOrder.MapToMandateRelatedInformation(),
            RemittanceInformation = standingOrder.Reference is not null
                ? new OBRemittanceInformation2
                {
                    Structured =
                    [
                        new OBRemittanceInformationStructured
                        {
                            CreditorReferenceInformation = new CreditorReferenceInformation
                            {
                                Reference = standingOrder.Reference
                            }
                        }
                    ]
                }
                : null
        };

    private static OBMandateRelatedInformation1 MapToMandateRelatedInformation(
        this AccountAndTransactionModelsV3p1p11.OBStandingOrder6 standingOrder) =>
        new()
        {
            MandateIdentification = null, // not in v3
            Classification = null, // not in v3
            CategoryPurposeCode = null, // not in v3
            FirstPaymentDateTime = standingOrder.FirstPaymentDateTime,
            RecurringPaymentDateTime = null, // not in v3
            FinalPaymentDateTime = standingOrder.FinalPaymentDateTime,
            Frequency = new OBFrequency6
            {
                Type = standingOrder.Frequency,
                CountPerPeriod = null, // not in v3
                PointInTime = null // not in v3
                //AdditionalProperties
            },
            Reason = null // not in v3
            //AdditionalProperties
        };

    private static ExternalMandateStatus1Code MapToStandingOrderStatusCode(
        this AccountAndTransactionModelsV3p1p11.OBExternalStandingOrderStatus1Code status) =>
        status switch
        {
            AccountAndTransactionModelsV3p1p11.OBExternalStandingOrderStatus1Code.Active => ExternalMandateStatus1Code
                .ACTV,
            AccountAndTransactionModelsV3p1p11.OBExternalStandingOrderStatus1Code.Inactive => ExternalMandateStatus1Code
                .SUSP, // decision: use SUSP ("put on hold")
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
}
