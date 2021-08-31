// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.ExternalApiBase;
using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Validators;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models
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

    public partial class OBWriteFundsConfirmationResponse1 : ISupportsValidation
    {
        public async Task<ValidationResult> ValidateAsync() =>
            await new OBWriteFundsConfirmationResponse1Validator()
                .ValidateAsync(this)!;
    }
}
