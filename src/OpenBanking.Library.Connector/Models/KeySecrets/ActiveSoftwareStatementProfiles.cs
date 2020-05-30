// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets
{
    public class ActiveSoftwareStatementProfiles : IKeySecretItem
    {
        public ActiveSoftwareStatementProfiles(List<string> profileIds)
        {
            ProfileIds = profileIds ?? throw new ArgumentNullException(nameof(profileIds));
        }

        public List<string> ProfileIds { get; }
    }
}
