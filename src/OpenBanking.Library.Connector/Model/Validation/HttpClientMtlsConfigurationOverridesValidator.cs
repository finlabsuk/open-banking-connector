// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Validation
{
    public class HttpClientMtlsConfigurationOverridesValidator : AbstractValidator<HttpClientMtlsConfigurationOverrides>
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
                .WithMessage($"Invalid {nameof(HttpClientMtlsConfigurationOverrides.TlsCertificateVerification)}.");

            RuleFor(x => x.TlsRenegotiationSupport)
                .Must(ValidationRules.IsNotNullOrEmpty)
                .WithMessage($"Invalid {nameof(HttpClientMtlsConfigurationOverrides.TlsRenegotiationSupport)}.");
        }
    }
}
