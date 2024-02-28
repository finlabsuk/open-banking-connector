// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.Repositories;

public class ProcessedTransportCertificateProfiles
{
    public ProcessedTransportCertificateProfiles(
        ObWacCertificate defaultVariant,
        ConcurrentDictionary<string, ObWacCertificate> overrideVariants)
    {
        DefaultVariant = defaultVariant;
        OverrideVariants = overrideVariants;
    }

    public ObWacCertificate DefaultVariant { get; }

    public ConcurrentDictionary<string, ObWacCertificate> OverrideVariants { get; }
}
