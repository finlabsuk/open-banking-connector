// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AutoMapper;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModel.PaymentInitiation.V3p1p1.Model;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Converters
{
    public class
        OBWriteDomesticConsentDataConverter : ITypeConverter<OBWriteDomesticConsentData, OBWriteDataDomesticConsent2>
    {
        public OBWriteDataDomesticConsent2 Convert(OBWriteDomesticConsentData source,
            OBWriteDataDomesticConsent2 destination,
            ResolutionContext context)
        {
            var init = context.Mapper.Map<OBDomestic2>(source.Initiation);
            var auth = context.Mapper.Map<OBAuthorisation1>(source.Authorisation);

            return new OBWriteDataDomesticConsent2(init, auth);
        }
    }
}
