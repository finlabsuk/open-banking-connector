// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p6.PaymentInitiation.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response
{
    public interface IDomesticPaymentPublicQuery
    {
        PaymentInitiationModelsPublic.OBWriteDomesticResponse5 OBWriteDomesticResponse { get; }
    }

    /// <summary>
    ///     Respnose to GetLocal
    /// </summary>
    public class DomesticPaymentGetLocalResponse : IDomesticPaymentPublicQuery
    {
        internal DomesticPaymentGetLocalResponse(
            PaymentInitiationModelsPublic.OBWriteDomesticResponse5 obWriteDomesticResponse)
        {
            OBWriteDomesticResponse = obWriteDomesticResponse;
        }

        public PaymentInitiationModelsPublic.OBWriteDomesticResponse5 OBWriteDomesticResponse { get; }
    }

    /// <summary>
    ///     Response to Post
    /// </summary>
    public class DomesticPaymentPostResponse : DomesticPaymentGetLocalResponse
    {
        internal DomesticPaymentPostResponse(
            PaymentInitiationModelsPublic.OBWriteDomesticResponse5 obWriteDomesticResponse) : base(
            obWriteDomesticResponse) { }
    }
}
