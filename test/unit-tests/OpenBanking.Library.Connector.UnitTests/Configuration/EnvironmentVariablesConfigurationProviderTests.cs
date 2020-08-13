// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FsCheck;
using FsCheck.Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Configuration
{
    public class EnvironmentVariablesConfigurationProviderTests
    {
        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property GetRuntimeConfiguration_SoftwareIdInherited(string value)
        {
            IDictionary evs = new Hashtable();

            Func<bool> rule = () =>
            {
                ObcConfiguration? config = new EnvironmentVariablesConfigurationProvider(evs).GetObcConfiguration();

                return config.SoftwareId != value;
            };

            return rule.When(!String.IsNullOrEmpty(value));
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property GetRuntimeConfiguration_SoftwareIdApplied(string value)
        {
            IDictionary evs = new Hashtable();
            evs["OpenBankingConnector:SoftwareId"] = value;

            Func<bool> rule = () =>
            {
                ObcConfiguration? config = new EnvironmentVariablesConfigurationProvider(evs).GetObcConfiguration();

                return config.SoftwareId == value;
            };

            return rule.When(value != null);
        }


        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property GetRuntimeConfiguration_DefaultCurrencyInherited(string value)
        {
            IDictionary evs = new Hashtable();

            Func<bool> rule = () =>
            {
                ObcConfiguration? config = new EnvironmentVariablesConfigurationProvider(evs).GetObcConfiguration();

                return config.DefaultCurrency != value;
            };

            return rule.When(value != null);
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property GetRuntimeConfiguration_DefaultCurrencyApplied(string value)
        {
            IDictionary evs = new Hashtable();
            evs["OpenBankingConnector:DefaultCurrency"] = value;

            Func<bool> rule = () =>
            {
                ObcConfiguration? config = new EnvironmentVariablesConfigurationProvider(evs).GetObcConfiguration();

                return config.DefaultCurrency == value;
            };

            return rule.When(value != null);
        }
    }
}
