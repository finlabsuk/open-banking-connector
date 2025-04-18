﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;

public abstract class GrantPostCustomBehaviour
{
    public bool? ResponseScopeMayIncludeExtraValues { get; set; }

    public bool? ResponseScopeEmptyTreatedAsNull { get; set; }

    public bool? ResponseTokenTypeCaseMayBeIncorrect { get; set; }
}
