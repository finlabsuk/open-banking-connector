// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p6.PaymentInitiation.Models;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p6.PaymentInitiation.Validators
{
    public class
        OBWriteDomesticResponse5DataValidator : AbstractValidator<
            OBWriteDomesticResponse5Data>
    {
        public OBWriteDomesticResponse5DataValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.Initiation)
                .SetValidator(new OBWriteDomesticResponse5DataInitiationValidator());
        }
    }
}
