// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AutoMapper;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent.PaymentInitiation;
using OBWriteDomesticConsent =
    FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation.OBWriteDomesticConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Converters.Persistent
{
    public class OBWriteDomesticConsentConverter : ITypeConverter<OBWriteDomesticConsent, DomesticConsent>
    {
        public DomesticConsent Convert(OBWriteDomesticConsent source, DomesticConsent destination,
            ResolutionContext context)
        {
            return new DomesticConsent
            {
                Id = source.OpenBankingClientProfileId,

                ObWriteDomesticConsent = new Model.Persistent.PaymentInitiation.OBWriteDomesticConsent
                {
                    Data = context.Mapper.Map<OBWriteDomesticConsentData>(source.Data),
                    OpenBankingClientProfileId = source.OpenBankingClientProfileId,
                    Risk = context.Mapper.Map<OBRisk>(source.Risk)
                },
                IssuerUrl = "TODO",
                SoftwareStatementProfileId = "TODO",
                OpenBankingClientProfileId = source.OpenBankingClientProfileId,
                State = "TODO"
            };
        }
    }
}
