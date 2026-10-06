// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.PaymentInitiation;

public delegate ConcurrentDictionary<string, string> GetDomesticPaymentConsentAuthExtraParameters(
    DomesticPaymentConsentAuthContext request);

public class DomesticPaymentConsentAuthGetCustomBehaviour : ConsentAuthGetCustomBehaviour
{
    public GetDomesticPaymentConsentAuthExtraParameters? GetExtraParameters { get; init; }
}
