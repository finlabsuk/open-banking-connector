﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;

public class AccountAndTransactionApi
{
    private readonly string _baseUrl = null!;
    public AccountAndTransactionApiVersion ApiVersion { get; init; } = AccountAndTransactionApiVersion.Version3p1p11;

    public required string BaseUrl
    {
        get => _baseUrl;
        init => _baseUrl = value.TrimEnd('/');
    }
}
