// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validators
{
    public class HttpClientMtlsConfigurationOverridesValidator : AbstractValidator<HttpMtlsConfigurationOverrides>
    {
        public HttpClientMtlsConfigurationOverridesValidator()
        {
            CascadeMode = CascadeMode.Continue;
            CreateRules();
        }

        private void CreateRules()
        {
            RuleFor(x => x.TlsCertificateVerification)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .WithMessage($"Invalid {nameof(HttpMtlsConfigurationOverrides.TlsCertificateVerification)}.");

            RuleFor(x => x.TlsRenegotiationSupport)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .WithMessage($"Invalid {nameof(HttpMtlsConfigurationOverrides.TlsRenegotiationSupport)}.");
        }
    }
}
