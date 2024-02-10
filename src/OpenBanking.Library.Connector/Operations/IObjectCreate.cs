// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal class LocalCreateParams { }

internal class BankRegistrationCreateParams : LocalCreateParams { }

internal class ConsentCreateParams : LocalCreateParams
{
    public string? PublicRequestUrlWithoutQuery { get; init; }
}

internal class VrpConsentFundsConfirmationCreateParams : ConsentCreateParams
{
    public required Guid Id { get; init; }
}

internal interface IObjectCreate<in TPublicRequest, TPublicResponse, in TCreateParams>
    where TCreateParams : LocalCreateParams
{
    Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> CreateAsync(
        TPublicRequest request,
        TCreateParams createParams);
}

internal interface IVrpConsentFundsConfirmationCreate<in TPublicRequest, TPublicResponse>
{
    Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        CreateFundsConfirmationAsync(
            TPublicRequest request,
            VrpConsentFundsConfirmationCreateParams createParams);
}
