// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using Microsoft.Extensions.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Configuration;

public class SecretProvider : ISecretProvider
{
    private readonly IConfiguration _configuration;

    public SecretProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetSecret(string name)
    {
        var value = _configuration.GetValue<string>(name, "");
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"Cannot get non-empty value from specified configuration setting {name}.");
        }

        return value;
    }

    public bool TryGetSecret(string name, [MaybeNullWhen(false)] out string value)
    {
        var tmp = _configuration.GetValue<string>(name, "");
        if (string.IsNullOrEmpty(tmp))
        {
            value = default;
            return false;
        }
        value = tmp;
        return true;
    }
}
