﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.BankUiMethods;

public class MonzoUiMethods : IBankUiMethods
{
    private readonly MonzoBank _monzoBank;

    public MonzoUiMethods(MonzoBank monzoBank)
    {
        _monzoBank = monzoBank;
    }

    public Task PerformConsentAuthUiInteractions(
        ConsentVariety consentVariety,
        IPage page,
        BankUser bankUser) =>
        Task.CompletedTask;
}
