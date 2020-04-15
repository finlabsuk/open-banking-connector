// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Entities
{
    internal static class MessagesResponseExtensions
    {
        public static MessagesResponse ToMessagesResponse(this FluentResponse value)
        {
            if (value.Messages != null)
            {
                var result = new MessagesResponse();

                var infos = value.Messages.OfType<FluentResponseInfoMessage>().Select(m => m.Message).ToList();
                if (infos.Count > 0)
                {
                    result.InformationMessages = infos;
                }

                var warnings = value.Messages.OfType<FluentResponseWarningMessage>().Select(m => m.Message)
                    .ToList();
                if (warnings.Count > 0)
                {
                    result.WarningMessages = warnings;
                }

                var errors = value.Messages.OfType<FluentResponseErrorMessage>().Select(m => m.Message).ToList();
                if (errors.Count > 0)
                {
                    result.ErrorMessages = errors;
                }

                return result;
            }

            return null;
        }
    }
}
