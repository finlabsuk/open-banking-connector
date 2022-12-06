// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;

namespace FinnovationLabs.OpenBanking.Library.Connector.Repositories;

public class ProcessedSoftwareStatementProfiles
{
    public ProcessedSoftwareStatementProfiles(
        ProcessedSoftwareStatementProfile defaultVariant,
        ConcurrentDictionary<string, ProcessedSoftwareStatementProfile> overrideVariants)
    {
        DefaultVariant = defaultVariant;
        OverrideVariants = overrideVariants;
    }

    public ProcessedSoftwareStatementProfile DefaultVariant { get; }

    public ConcurrentDictionary<string, ProcessedSoftwareStatementProfile> OverrideVariants { get; }
}
