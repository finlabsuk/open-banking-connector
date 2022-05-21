// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Runtime.Serialization;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests
{
    public class LocalFileTestConfigurationProvider : ITestConfigurationProvider
    {
        private readonly TestConfiguration? _configuration;

        public LocalFileTestConfigurationProvider(string? filePrefix)
        {
            if (string.IsNullOrEmpty(filePrefix))
            {
                filePrefix = "local";
            }

            _configuration = ReadConfigurationFile(filePrefix);
        }

        public bool? GetBooleanValue(string key)
        {
            if (_configuration == null)
            {
                return null;
            }

            switch (key.ToLower())
            {
                case "mockhttp":
                    return _configuration.MockHttp != null
                        ? bool.Parse(_configuration.MockHttp)
                        : null;
                default:
                    return null;
            }
        }

        public string? GetValue(string key)
        {
            if (_configuration == null)
            {
                return null;
            }

            switch (key.ToLower())
            {
                case "softwarestatement":
                    return _configuration.SoftwareStatement;
                case "signingkeyid":
                    return _configuration.SigningKeyId;
                case "signingcertificatekey":
                    return _configuration.SigningCertificateKey;
                case "signingcertificate":
                    return _configuration.SigningCertificate;
                case "transportcertificatekey":
                    return _configuration.TransportCertificateKey;
                case "transportcertificate":
                    return _configuration.TransportCertificate;
                case "defaultfragmentredirecturl":
                    return _configuration.DefaultFragmentRedirectUrl;
                case "xfapifinancialid":
                    return _configuration.XFapiFinancialId;
                case "clientprofileissuerurl":
                    return _configuration.ClientProfileIssuerURl;
                case "accountapiurl":
                    return _configuration.AccountApiUrl;
                case "paymentapiurl":
                    return _configuration.PaymentApiUrl;
                default:
                    return null;
            }
        }

        public T? GetEnumValue<T>(string key)
            where T : struct
        {
            if (_configuration == null)
            {
                return null;
            }

            switch (key.ToLower())
            {
                case "accountapiversion":
                    return GetValueFromEnumMember<T>(_configuration.AccountApiVersion);
                case "paymentapiversion":
                    return GetValueFromEnumMember<T>(_configuration.PaymentApiVersion);
                default:
                    return null;
            }
        }

        public BankRegistrationPostCustomBehaviour? GetOpenBankingClientRegistrationClaimsOverrides()
        {
            if (_configuration == null)
            {
                return null;
            }

            return _configuration.OpenBankingClientRegistrationClaimsOverrides;
        }

        public OpenIdConfiguration? GetOpenBankingOpenIdConfiguration()
        {
            if (_configuration == null)
            {
                return null;
            }

            return _configuration.OpenIdConfiguration;
        }

        private TestConfiguration? ReadConfigurationFile(string filePrefix)
        {
            string path = GetConfigurationFilePath(filePrefix);
            if (File.Exists(path))
            {
                string? json;

                using (var stream = new FileStream(
                           path,
                           FileMode.Open,
                           FileAccess.Read,
                           FileShare.Read))
                {
                    using (var rdr = new StreamReader(stream))
                    {
                        json = rdr.ReadToEnd();
                    }
                }

                return JsonConvert.DeserializeObject<TestConfiguration>(json);
            }

            return null;
        }

        private string GetConfigurationFilePath(string filePrefix)
        {
            string path = Assembly.GetExecutingAssembly().Location;
            path = Path.GetDirectoryName(path)!;

            var fileName = "test.settings.json";

            if (!string.IsNullOrEmpty(filePrefix))
            {
                fileName = filePrefix + "." + fileName;
            }

            return Path.Combine(path, fileName);
        }

        private T? GetValueFromEnumMember<T>(string value)
            where T : struct
        {
            Type type = typeof(T);
            if (type.GetTypeInfo().IsEnum)
            {
                foreach (string name in Enum.GetNames(type))
                {
                    var attr = type.GetRuntimeField(name)
                        ?.GetCustomAttribute<EnumMemberAttribute>(true);
                    if (attr != null &&
                        attr.Value == value)
                    {
                        return (T?) Enum.Parse(type, name);
                    }
                }

                return default;
            }

            throw new InvalidOperationException($"{value} is not a member of {type.Name}.");
        }
    }
}
