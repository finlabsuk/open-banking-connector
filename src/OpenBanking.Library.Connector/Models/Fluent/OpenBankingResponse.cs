// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent
{
    public abstract class OpenBankingResponse
    {
        protected OpenBankingResponse() : this(new List<OpenBankingResponseMessage>())
        {
        }

        protected OpenBankingResponse(IList<OpenBankingResponseMessage> messages)
        {
            Messages = messages ?? new List<OpenBankingResponseMessage>();
        }


        public IList<OpenBankingResponseMessage> Messages { get; }

        public bool HasInfos => Messages.NullToEmpty().OfType<OpenBankingResponseInfoMessage>().Any();
        public bool HasWarnings => Messages.NullToEmpty().OfType<OpenBankingResponseWarningMessage>().Any();
        public bool HasErrors => Messages.NullToEmpty().OfType<OpenBankingResponseErrorMessage>().Any();
    }
}
