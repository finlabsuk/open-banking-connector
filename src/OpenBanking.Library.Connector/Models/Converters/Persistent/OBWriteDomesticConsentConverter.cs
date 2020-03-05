// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AutoMapper;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using OBWriteDomesticConsent =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.OBWriteDomesticConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Converters.Persistent
{
    public class OBWriteDomesticConsentConverter : ITypeConverter<Public.PaymentInitiation.OBWriteDomesticConsent, DomesticConsent>
    {
        public DomesticConsent Convert(OBWriteDomesticConsent source, DomesticConsent destination,
            ResolutionContext context)
        {
            return new DomesticConsent
            {
                Id = source.ApiProfileId,

                ObWriteDomesticConsent = new Models.Persistent.PaymentInitiation.OBWriteDomesticConsent
                {
                    Data = context.Mapper.Map<OBWriteDomesticConsentData>(source.Data),
                    ApiProfileId = source.ApiProfileId,
                    Risk = context.Mapper.Map<OBRisk>(source.Risk)
                },
                IssuerUrl = "TODO",
                SoftwareStatementProfileId = "TODO",
                ApiProfileId = source.ApiProfileId,
                State = "TODO"
            };
        }
    }
}
