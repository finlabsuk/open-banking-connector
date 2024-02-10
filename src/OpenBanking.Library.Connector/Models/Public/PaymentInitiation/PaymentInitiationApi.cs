// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

public class PaymentInitiationApi
{
    private readonly string _baseUrl = null!;
    public PaymentInitiationApiVersion ApiVersion { get; init; } = PaymentInitiationApiVersion.VersionPublic;

    public required string BaseUrl
    {
        get => _baseUrl;
        init => _baseUrl = value.TrimEnd('/');
    }
}
