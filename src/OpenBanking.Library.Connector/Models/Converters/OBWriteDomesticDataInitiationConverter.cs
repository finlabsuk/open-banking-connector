// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AutoMapper;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Converters
{
    public class OBWriteDomesticDataInitiationConverter : ITypeConverter<OBWriteDomesticDataInitiation, OBDomestic2>
    {
        public OBDomestic2 Convert(OBWriteDomesticDataInitiation source, OBDomestic2 destination,
            ResolutionContext context)
        {
            var amount = context.Mapper.Map<OBInternational2InstructedAmount>(source.InstructedAmount);
            var debtor = context.Mapper.Map<OBCashAccountDebtor4>(source.DebtorAccount);
            var creditor = context.Mapper.Map<OBCashAccountCreditor3>(source.CreditorAccount);
            var postalAddress = context.Mapper.Map<OBPostalAddress6>(source.CreditorPostalAddress);

            return new OBDomestic2(source.InstructionIdentification, source.EndToEndIdentification,
                source.LocalInstrument, amount,
                debtor, creditor, postalAddress);
        }
    }
}
