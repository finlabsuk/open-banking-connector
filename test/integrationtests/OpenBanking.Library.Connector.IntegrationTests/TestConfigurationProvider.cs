// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests
{
    public class TestConfigurationProvider : ITestConfigurationProvider
    {
        private readonly ITestConfigurationProvider[] _providers;

        public TestConfigurationProvider()
        {
            _providers = new[]
            {
                new LocalFileTestConfigurationProvider("user"),
                new LocalFileTestConfigurationProvider(null)
            };
        }

        public bool? GetBooleanValue(string key)
        {
            var results = _providers.Select(p => p.GetBooleanValue(key))
                .Where(v => v.HasValue);

            return results.FirstOrDefault();
        }

        public string GetValue(string key)
        {
            var results = _providers.Select(p => p.GetValue(key)).Where(v => v != null);

            return results.FirstOrDefault();
        }

        public T? GetEnumValue<T>(string key) where T : struct
        {
            var results = _providers.Select(p => p.GetEnumValue<T>(key)).Where(v => v.HasValue);

            return results.FirstOrDefault();
        }

        public OpenBankingClientRegistrationClaimsOverrides GetOpenBankingClientRegistrationClaimsOverrides()
        {
            var results = _providers.Select(p => p.GetOpenBankingClientRegistrationClaimsOverrides())
                .Where(v => v != null);

            // Populate default instance
            return results.FirstOrDefault() ?? new OpenBankingClientRegistrationClaimsOverrides();
        }

        public OpenIdConfiguration GetOpenBankingOpenIdConfiguration()
        {
            var results = _providers.Select(p => p.GetOpenBankingOpenIdConfiguration())
                .Where(v => v != null);

            return results.FirstOrDefault() ?? new OpenIdConfiguration
            {
                Issuer = "issuser name",
                ResponseModesSupported = new string[0],
                ScopesSupported = new string[0],
                ResponseTypesSupported = new string[0],
                AuthorizationEndpoint = "https://authorization_endpoint",
                RegistrationEndpoint = "https://registration_endpoint",
                TokenEndpoint = "https://token_endpoint"
            };
        }
    }
}
