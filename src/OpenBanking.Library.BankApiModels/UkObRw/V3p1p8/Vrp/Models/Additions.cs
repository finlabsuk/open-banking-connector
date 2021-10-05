// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Validators;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models
{
    public partial class OBDomesticVRPConsentRequest : ISupportsValidation
    {
        public async Task<ValidationResult> ValidateAsync() =>
            await new OBDomesticVRPConsentRequestValidator()
                .ValidateAsync(this)!;
    }

    public partial class OBDomesticVRPConsentResponse : ISupportsValidation
    {
        public async Task<ValidationResult> ValidateAsync() =>
            await new OBDomesticVRPConsentResponseValidator()
                .ValidateAsync(this)!;
    }

    public partial class OBVRPFundsConfirmationResponse : ISupportsValidation
    {
        public async Task<ValidationResult> ValidateAsync() =>
            await new OBVRPFundsConfirmationResponseValidator()
                .ValidateAsync(this)!;
    }

    public partial class OBDomesticVRPRequest : ISupportsValidation
    {
        public async Task<ValidationResult> ValidateAsync() =>
            await new OBDomesticVRPRequestValidator()
                .ValidateAsync(this)!;
    }

    public partial class OBDomesticVRPResponse : ISupportsValidation
    {
        public async Task<ValidationResult> ValidateAsync() =>
            await new OBDomesticVRPResponseValidator()
                .ValidateAsync(this)!;
    }
}
