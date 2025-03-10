﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;

namespace FinnovationLabs.OpenBanking.Library.Connector.Metrics;

public class TppReportingRequestInfo
{
    public required string EndpointDescription { get; init; }

    public required BankProfileEnum BankProfile { get; init; }
}
