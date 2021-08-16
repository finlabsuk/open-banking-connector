// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.ExternalApiBase;
using FinnovationLabs.OpenBanking.Library.Connector.UkDcrApi.V3p3.Validators;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.UkDcrApi.V3p3.Models
{
    public partial class OBClientRegistration1 : ISupportsValidation
    {
        public virtual async Task<ValidationResult> ValidateAsync() =>
            await new ObRegistrationProperties1Validator()
                .ValidateAsync(this)!;
    }

    [SourceApiEquivalent(
        typeof(V3p2.Models.OBClientRegistration1),
        ValueMappingSourceMembers = new string[]
        {
            null,
            null,
            null,
            null,
        },
        ValueMappingDestinationMembers = new[]
        {
            "BackchannelTokenDeliveryMode",
            "BackchannelClientNotificationEndpoint",
            "BackchannelAuthenticationRequestSigningAlg",
            "BackchannelUserCodeParameterSupported"
        },
        ValueMappings = new[]
        {
            ValueMapping.SetNull,
            ValueMapping.SetNull,
            ValueMapping.SetNull,
            ValueMapping.SetNull,
        })
    ]
    public partial class OBClientRegistration1Response : OBClientRegistration1
    {
        public override async Task<ValidationResult> ValidateAsync() =>
            await new OBClientRegistration1ResponseValidator()
                .ValidateAsync(this)!;
    }
}
