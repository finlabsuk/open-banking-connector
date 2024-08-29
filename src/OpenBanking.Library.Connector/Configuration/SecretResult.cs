// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Configuration;

public class SecretResult
{
    public required bool SecretObtained { get; init; }

    public string? Secret { get; init; }

    public string? ErrorMessage { get; init; }
}
