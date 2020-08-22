﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public class BankProfileContext
    {
        internal BankProfileContext(ISharedContext context)
        {
            Context = context;
        }

        internal ISharedContext Context { get; }

        internal BankProfile Data { get; set; } = new BankProfile();
        
    }
}
