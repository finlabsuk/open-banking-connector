// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

public class LocalCreateParams { }

internal class BankRegistrationCreateParams : LocalCreateParams { }

public class ConsentCreateParams : LocalCreateParams
{
    public required string? PublicRequestUrlWithoutQuery { get; init; }

    public required IEnumerable<HttpHeader>? ExtraHeaders { get; init; }
}

public class VrpConsentFundsConfirmationCreateParams : ConsentCreateParams
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
