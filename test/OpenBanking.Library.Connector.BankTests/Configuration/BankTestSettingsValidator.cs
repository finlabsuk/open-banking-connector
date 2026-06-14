// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Options;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration;

public class BankTestSettingsValidator : IValidateOptions<BankTestSettings>
{
    public ValidateOptionsResult Validate(string? name, BankTestSettings settings)
    {
        var failures = new List<string>();

        if (!settings.Auth.PlaywrightLaunch.IgnoreExecutablePathAndArgs)
        {
            string? executablePath = settings.Auth.PlaywrightLaunch.GetExecutablePathForCurrentOs();
            if (executablePath is null)
            {
                failures.Add("Please specify an executable path in app settings.");
            }
            else if (!File.Exists(executablePath))
            {
                failures.Add(
                    $"Can't locate executable path specified in bank test setting ExecutablePath: " +
                    $"{executablePath}. Please update app settings.");
            }
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
