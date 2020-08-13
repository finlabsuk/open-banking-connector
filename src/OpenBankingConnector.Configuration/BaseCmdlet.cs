// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Management.Automation;

namespace OpenBankingConnector.Configuration
{
    public class BaseCmdlet : Cmdlet
    {
        private readonly string _verbName;
        private readonly string _nounName;

        public BaseCmdlet(string verbName, string nounName)
        {
            _verbName = nounName ?? throw new ArgumentNullException(nameof(verbName));
            _nounName = nounName ?? throw new ArgumentNullException(nameof(nounName));
        }

        protected override void BeginProcessing()
        {
            WriteVerbose($"{_verbName}-{_nounName}: start");
        }

        protected override void EndProcessing()
        {
            WriteVerbose($"{_verbName}-{_nounName}: end");
        }
    }
}
