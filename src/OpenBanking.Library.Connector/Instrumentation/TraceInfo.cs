// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace FinnovationLabs.OpenBanking.Library.Connector.Instrumentation
{
    public class TraceInfo
    {
        private readonly Dictionary<string, string> _values;

        public TraceInfo(string message)
        {
            Message = message;
            _values = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        public string Message { get; }

        public IEnumerable<KeyValuePair<string, string>> Values => _values;

        public TraceInfo Add(string name, string value)
        {
            _values[name] = value;

            return this;
        }
    }
}
