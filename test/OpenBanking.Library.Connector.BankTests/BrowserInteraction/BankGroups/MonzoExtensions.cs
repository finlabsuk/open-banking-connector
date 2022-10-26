// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.BankGroups;

public static class MonzoExtensions
{
    public static Task ConsentUiInteractions(
        this Monzo bankGroup,
        BankProfileEnum bankProfileEnum,
        IPage page,
        ConsentVariety consentVariety,
        BankUser bankUser)
    {
        return Task.CompletedTask;
    }
}
