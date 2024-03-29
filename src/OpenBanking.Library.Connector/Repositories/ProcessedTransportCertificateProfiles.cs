﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;

namespace FinnovationLabs.OpenBanking.Library.Connector.Repositories;

public class ProcessedTransportCertificateProfiles
{
    public ProcessedTransportCertificateProfiles(
        ProcessedTransportCertificateProfile defaultVariant,
        ConcurrentDictionary<string, ProcessedTransportCertificateProfile> overrideVariants)
    {
        DefaultVariant = defaultVariant;
        OverrideVariants = overrideVariants;
    }

    public ProcessedTransportCertificateProfile DefaultVariant { get; }

    public ConcurrentDictionary<string, ProcessedTransportCertificateProfile> OverrideVariants { get; }
}
