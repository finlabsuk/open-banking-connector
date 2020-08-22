// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request
{
    public class DomesticPayment
    {
        public string ConsentId { get; set; } = null!;
    }
}
