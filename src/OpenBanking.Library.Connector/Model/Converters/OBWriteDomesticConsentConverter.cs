// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AutoMapper;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Converters
{
    public class OBWriteDomesticConsentConverter : ITypeConverter<OBWriteDomesticConsent, OBWriteDomesticConsent2>
    {
        public OBWriteDomesticConsent2 Convert(OBWriteDomesticConsent source, OBWriteDomesticConsent2 destination,
            ResolutionContext context)
        {
            var d2 = context.Mapper.Map<OBWriteDataDomesticConsent2>(source.Data);
            var r2 = context.Mapper.Map<OBRisk1>(source.Risk);

            return new OBWriteDomesticConsent2(d2, r2);
        }
    }

    public class
        OBWriteDomesticConsentOBWriteDomestic2Converter : ITypeConverter<OBWriteDomesticConsent, OBWriteDomestic2>
    {
        public OBWriteDomestic2 Convert(OBWriteDomesticConsent source, OBWriteDomestic2 destination,
            ResolutionContext context)
        {
            var r2 = context.Mapper.Map<OBRisk1>(source.Risk);


            var initiation = context.Mapper.Map<OBDomestic2>(source.Data.Initiation);
            var consentId = ""; // placeholder. Nulls cause ctor exceptions

            var d2 = new OBWriteDataDomestic2(consentId, initiation);

            return new OBWriteDomestic2(d2, r2);
        }
    }
}
