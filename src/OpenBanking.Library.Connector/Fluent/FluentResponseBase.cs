﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public abstract class FluentResponseBase
    {
        protected FluentResponseBase() : this(new List<FluentResponseMessage>()) { }

        protected FluentResponseBase(IList<FluentResponseMessage> messages)
        {
            Messages = messages ?? new List<FluentResponseMessage>();
        }

        public IList<FluentResponseMessage> Messages { get; }

        public bool HasInfos => Messages.NullToEmpty().OfType<FluentResponseInfoMessage>().Any();
        public bool HasWarnings => Messages.NullToEmpty().OfType<FluentResponseWarningMessage>().Any();
        public bool HasErrors => Messages.NullToEmpty().OfType<FluentResponseErrorMessage>().Any();
    }
}
