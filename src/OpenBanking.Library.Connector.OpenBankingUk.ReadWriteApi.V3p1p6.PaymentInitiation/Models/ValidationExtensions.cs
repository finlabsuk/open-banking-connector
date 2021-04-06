// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.ApiModels.Base;
using FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p6.PaymentInitiation.Validators;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p6.PaymentInitiation.Models
{
    public partial class OBWriteDomestic2 : ISupportsValidation
    {
        public async Task<ValidationResult> ValidateAsync() =>
            await new OBWriteDomestic2Validator()
                .ValidateAsync(this)!;
    }

    public partial class OBWriteDomesticResponse5 : ISupportsValidation
    {
        public async Task<ValidationResult> ValidateAsync() =>
            await new OBWriteDomesticResponse5Validator()
                .ValidateAsync(this)!;
    }

    public partial class OBWriteDomesticConsent4 : ISupportsValidation
    {
        public async Task<ValidationResult> ValidateAsync() =>
            await new OBWriteDomesticConsent4Validator()
                .ValidateAsync(this)!;
    }

    public partial class OBWriteDomesticConsentResponse5 : ISupportsValidation
    {
        public async Task<ValidationResult> ValidateAsync() =>
            await new OBWriteDomesticConsentResponse5Validator()
                .ValidateAsync(this)!;
    }
}
