﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation
{
    public class PaymentInitiationApi
    {
        public PaymentInitiationApiVersion PaymentInitiationApiVersion { get; set; }

        public string BaseUrl { get; set; } = null!;
    }
}
