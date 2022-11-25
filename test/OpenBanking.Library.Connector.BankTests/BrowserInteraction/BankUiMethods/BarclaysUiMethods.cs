// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.BankUiMethods;

public class BarclaysUiMethods : IBankUiMethods
{
    private readonly BarclaysBank _barclaysBank;

    public BarclaysUiMethods(BarclaysBank barclaysBank)
    {
        _barclaysBank = barclaysBank;
    }

    public Task PerformConsentAuthUiInteractions(
        ConsentVariety consentVariety,
        IPage page,
        BankUser bankUser)
    {
        if (_barclaysBank is BarclaysBank.Sandbox)
        {
        }

        return Task.CompletedTask;
    }
}
