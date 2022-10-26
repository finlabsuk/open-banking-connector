// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;

public interface IBankGroupUiMethods
{
    Task PerformConsentAuthUiInteractions(
        BankProfileEnum bankProfileEnum,
        ConsentVariety consentVariety,
        IPage page,
        BankUser bankUser);

    bool RequiresManualInteraction(BankProfileEnum bankProfileEnum, ConsentVariety consentVariety);
}
