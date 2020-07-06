// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AutoMapper;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Converters
{
    public class OBWriteDataDomestic2Converter : ITypeConverter<OBWriteDomesticConsentData, OBWriteDataDomestic2>
    {
        public OBWriteDataDomestic2 Convert(
            OBWriteDomesticConsentData source,
            OBWriteDataDomestic2 destination,
            ResolutionContext context)
        {
            OBDomestic2 initiation = context.Mapper.Map<OBDomestic2>(source.Initiation);
            string consentId = "<Placeholder>"; // placeholder. Nulls cause ctor exceptions

            return new OBWriteDataDomestic2(consentId: consentId, initiation: initiation);
        }
    }
}
