﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.Sandbox;

public class Hsbc2 : IBankProfileUiMethods
{
    public Task ConsentUiInteractions(IPage page, ConsentVariety consentVariety, BankUser bankUser)
    {
        return Task.CompletedTask;
    }
}