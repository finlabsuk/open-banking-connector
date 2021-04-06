// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p6.PaymentInitiation.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response
{
    public interface IDomesticPaymentConsentPublicQuery
    {
        Guid Id { get; }
        PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5 OBWriteDomesticConsentResponse { get; }

        Guid BankRegistrationId { get; }

        Guid BankApiInformationId { get; }
    }

    /// <summary>
    ///     Respnose to GetLocal
    /// </summary>
    public class DomesticPaymentConsentGetLocalResponse : IDomesticPaymentConsentPublicQuery
    {
        internal DomesticPaymentConsentGetLocalResponse(
            Guid id,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5 obWriteDomesticConsentResponse,
            Guid bankRegistrationId,
            Guid bankApiInformationId)
        {
            Id = id;
            OBWriteDomesticConsentResponse = obWriteDomesticConsentResponse;
            BankRegistrationId = bankRegistrationId;
            BankApiInformationId = bankApiInformationId;
        }

        public Guid Id { get; }

        public PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5 OBWriteDomesticConsentResponse
        {
            get;
            set;
        }

        public Guid BankRegistrationId { get; }
        public Guid BankApiInformationId { get; }
    }

    public class DomesticPaymentConsentGetResponse : DomesticPaymentConsentGetLocalResponse
    {
        internal DomesticPaymentConsentGetResponse(
            Guid id,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5 obWriteDomesticConsentResponse,
            Guid bankRegistrationId,
            Guid bankApiInformationId) : base(
            id,
            obWriteDomesticConsentResponse,
            bankRegistrationId,
            bankApiInformationId) { }
    }

    /// <summary>
    ///     Response to Post
    /// </summary>
    public class DomesticPaymentConsentPostResponse : DomesticPaymentConsentGetResponse
    {
        internal DomesticPaymentConsentPostResponse(
            string authUrl,
            Guid id,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5 obWriteDomesticConsentResponse,
            Guid bankRegistrationId,
            Guid bankApiInformationId) : base(
            id,
            obWriteDomesticConsentResponse,
            bankRegistrationId,
            bankApiInformationId)
        {
            AuthUrl = authUrl;
        }

        public string AuthUrl { get; }
    }
}
