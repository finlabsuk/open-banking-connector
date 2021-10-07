﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles
{
    public enum DomesticVrpTypeEnum
    {
        VrpWithDebtorAccountSpecifiedByPisp,
        VrpWithDebtorAccountSpecifiedDuringConsentAuthorisationAndCreditorAccountSpecifiedDuringPaymentInitiation
    }

    public partial class BankProfile
    {
        public DomesticVrpConsent DomesticVrpConsentRequest(
            Guid bankRegistrationId,
            Guid bankApiSetId,
            DomesticVrpTypeEnum domesticVrpType,
            string instructionIdentification,
            string endToEndIdentification,
            string? name)
        {
            var domesticVrpConsentRequest = new DomesticVrpConsent
            {
                OBDomesticVRPConsentRequest =
                    domesticVrpType switch
                    {
                        DomesticVrpTypeEnum.VrpWithDebtorAccountSpecifiedByPisp => new
                            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest
                            {
                                Data = new VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequestData
                                {
                                    ReadRefundAccount = VariableRecurringPaymentsModelsPublic
                                        .OBDomesticVRPConsentRequestDataReadRefundAccountEnum.No,
                                    ControlParameters =
                                        new VariableRecurringPaymentsModelsPublic.OBDomesticVRPControlParameters
                                        {
                                            ValidFromDateTime = DateTimeOffset.Now,
                                            //do now + 3 years - google
                                            ValidToDateTime = DateTimeOffset.Now,
                                            MaximumIndividualAmount =
                                                new VariableRecurringPaymentsModelsPublic.
                                                    OBDomesticVRPControlParametersMaximumIndividualAmount
                                                    {
                                                        Amount = "100.00",
                                                        Currency = "GBP"
                                                    },
                                            PeriodicLimits = null,
                                            VRPType = new List<string> { "UK.OBIE.VRPType.Sweeping" },
                                            PSUAuthenticationMethods = new List<string> { "UK.OBIE.SCA" },
                                            SupplementaryData = null
                                        },
                                    Initiation = new VariableRecurringPaymentsModelsPublic.OBDomesticVRPInitiation
                                    {
                                        DebtorAccount =
                                            new VariableRecurringPaymentsModelsPublic.OBCashAccountDebtorWithName
                                            {
                                                SchemeName = "UK.OBIE.IBAN",
                                                Identification = "GB76LOYD30949301273801",
                                                SecondaryIdentification = null,
                                                Name = "Marcus Sweepimus"
                                            },
                                        CreditorAgent = null,
                                        CreditorAccount =
                                            new VariableRecurringPaymentsModelsPublic.OBCashAccountCreditor3
                                            {
                                                SchemeName = "SortCodeAccountNumber",
                                                Identification = "30949330000010",
                                                SecondaryIdentification = "Roll 90210",
                                                Name = "Marcus Sweepimus"
                                            },
                                        RemittanceInformation =
                                            new VariableRecurringPaymentsModelsPublic.
                                                OBDomesticVRPInitiationRemittanceInformation
                                                {
                                                    Unstructured = null,
                                                    Reference = "Sweepco"
                                                }
                                    }
                                },
                                Risk = new VariableRecurringPaymentsModelsPublic.OBRisk1
                                {
                                    PaymentContextCode = VariableRecurringPaymentsModelsPublic
                                        .OBRisk1PaymentContextCodeEnum.PartyToParty,
                                    MerchantCategoryCode = null,
                                    MerchantCustomerIdentification = null,
                                    DeliveryAddress = null
                                }
                            },

                        DomesticVrpTypeEnum
                                .VrpWithDebtorAccountSpecifiedDuringConsentAuthorisationAndCreditorAccountSpecifiedDuringPaymentInitiation
                            =>
                            new VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest
                            {
                                Data = new VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequestData
                                {
                                    ReadRefundAccount = VariableRecurringPaymentsModelsPublic
                                        .OBDomesticVRPConsentRequestDataReadRefundAccountEnum.Yes,
                                    ControlParameters =
                                        new VariableRecurringPaymentsModelsPublic.OBDomesticVRPControlParameters
                                        {
                                            PSUAuthenticationMethods = new List<string> { "UK.OBIE.SCA" },
                                            VRPType = new List<string> { "UK.OBIE.VRPType.Sweeping" },
                                            ValidFromDateTime = DateTimeOffset.Now,
                                            ValidToDateTime = DateTimeOffset.Now,
                                            MaximumIndividualAmount =
                                                new VariableRecurringPaymentsModelsPublic.
                                                    OBDomesticVRPControlParametersMaximumIndividualAmount
                                                    {
                                                        Amount = "100.00",
                                                        Currency = "GBP"
                                                    },

                                            PeriodicLimits =
                                                new List<VariableRecurringPaymentsModelsPublic.
                                                    OBDomesticVRPControlParametersPeriodicLimitsItem>
                                                {
                                                    //this is supposed to have amount, currency, periodalignment, periodtype
                                                    // fix this 
                                                    new VariableRecurringPaymentsModelsPublic.OBDomesticVRPControlParametersPeriodicLimitsItem
                                                    {
                                                        PeriodType = VariableRecurringPaymentsModelsPublic
                                                            .OBDomesticVRPControlParametersPeriodicLimitsItemPeriodTypeEnum
                                                            .Day,
                                                        PeriodAlignment = VariableRecurringPaymentsModelsPublic
                                                            .OBDomesticVRPControlParametersPeriodicLimitsItemPeriodAlignmentEnum
                                                            .Consent,
                                                        Amount = null,
                                                        Currency = null
                                                    }



                                                },
                                        },
                                    Initiation = new VariableRecurringPaymentsModelsPublic.OBDomesticVRPInitiation
                                    {
                                        RemittanceInformation =
                                            new VariableRecurringPaymentsModelsPublic.
                                                OBDomesticVRPInitiationRemittanceInformation
                                                {
                                                    Unstructured = null,
                                                    Reference = "Sweepco"
                                                }
                                    }
                                },
                                Risk = new VariableRecurringPaymentsModelsPublic.OBRisk1
                                {
                                    PaymentContextCode = VariableRecurringPaymentsModelsPublic
                                        .OBRisk1PaymentContextCodeEnum.PartyToParty,
                                    MerchantCategoryCode = null,
                                    MerchantCustomerIdentification = null,
                                    DeliveryAddress = null
                                }
                            },
                        _ => throw new ArgumentOutOfRangeException(nameof(domesticVrpType), domesticVrpType, null)
                    },
                Name = name,
                BankApiSetId = bankApiSetId,
                BankRegistrationId = bankRegistrationId
            };
            return VariableRecurringPaymentsApiSettings.DomesticVrpConsentAdjustments(domesticVrpConsentRequest);
        }
    }
}