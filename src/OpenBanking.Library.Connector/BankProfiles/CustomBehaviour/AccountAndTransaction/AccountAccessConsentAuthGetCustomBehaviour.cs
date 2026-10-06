// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.AccountAndTransaction;

public delegate ConcurrentDictionary<string, string> GetAccountAccessConsentAuthExtraParameters(
    bool reAuthNotInitialAuth);

public class AccountAccessConsentAuthGetCustomBehaviour : ConsentAuthGetCustomBehaviour
{
    public GetAccountAccessConsentAuthExtraParameters? GetExtraParameters { get; init; }

    /// <summary>
    ///     Use consent to check for re-auth case (may be necessary if Open Banking Connector
    ///     does not know about previous successful auth)
    /// </summary>
    public bool? DetectReAuthCaseViaConsentStatus { get; init; }
}
