﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SourceApiEquivalentAttribute : ApiEquivalentAttribute
    {
        public SourceApiEquivalentAttribute(Type equivalentType) : base(equivalentType) { }
    }
}
