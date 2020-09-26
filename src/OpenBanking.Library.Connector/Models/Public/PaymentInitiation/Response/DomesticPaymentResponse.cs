// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using OBWriteDomesticResponse =
    FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model.OBWriteDomesticResponse4;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response
{
    public interface IDomesticPaymentPublicQuery
    {
        OBWriteDomesticResponse OBWriteDomesticResponse { get; }
    }

    public class DomesticPaymentResponse : IDomesticPaymentPublicQuery
    {
        internal DomesticPaymentResponse(OBWriteDomesticResponse obWriteDomesticResponse)
        {
            OBWriteDomesticResponse = obWriteDomesticResponse;
        }

        public OBWriteDomesticResponse OBWriteDomesticResponse { get; }
    }
}
