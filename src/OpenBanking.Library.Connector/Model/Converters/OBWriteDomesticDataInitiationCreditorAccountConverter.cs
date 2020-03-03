// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AutoMapper;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Converters
{
    public class
        OBWriteDomesticDataInitiationCreditorAccountConverter : ITypeConverter<
            OBWriteDomesticDataInitiationCreditorAccount, OBCashAccountCreditor3>
    {
        public OBCashAccountCreditor3 Convert(OBWriteDomesticDataInitiationCreditorAccount source,
            OBCashAccountCreditor3 destination,
            ResolutionContext context)
        {
            return new OBCashAccountCreditor3(source.SchemeName, source.Identification, source.Name,
                source.SecondaryIdentification);
        }
    }
}
