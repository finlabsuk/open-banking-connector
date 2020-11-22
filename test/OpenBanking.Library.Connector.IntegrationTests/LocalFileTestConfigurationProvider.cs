// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests
{
    public class LocalFileTestConfigurationProvider : ITestConfigurationProvider
    {
        private readonly TestConfiguration _configuration;

        public LocalFileTestConfigurationProvider(string filePrefix)
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
                        ? (bool?) bool.Parse(_configuration.MockHttp)
                        : null;
                default:
                    return null;
            }
        }

        public string GetValue(string key)
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
                    return GetValueFromEnumMember<T>(_configuration.AccountApiVersion);
                default:
                    return null;
            }
        }

        public BankRegistrationClaimsOverrides GetOpenBankingClientRegistrationClaimsOverrides()
        {
            if (_configuration == null)
            {
                return null;
            }

            return _configuration.OpenBankingClientRegistrationClaimsOverrides;
        }

        public OpenIdConfiguration GetOpenBankingOpenIdConfiguration()
        {
            if (_configuration == null)
            {
                return null;
            }

            return _configuration.OpenIdConfiguration;
        }

        private TestConfiguration ReadConfigurationFile(string filePrefix)
        {
            string path = GetConfigurationFilePath(filePrefix);
            if (File.Exists(path))
            {
                string json = null;

                using (FileStream? stream = new FileStream(
                    path: path,
                    mode: FileMode.Open,
                    access: FileAccess.Read,
                    share: FileShare.Read))
                {
                    using (StreamReader? rdr = new StreamReader(stream))
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
            string? path = Assembly.GetExecutingAssembly().Location;
            path = Path.GetDirectoryName(path);

            string fileName = "test.settings.json";

            if (!string.IsNullOrEmpty(filePrefix))
            {
                fileName = filePrefix + "." + fileName;
            }

            return Path.Combine(path1: path, path2: fileName);
        }

        private T GetValueFromEnumMember<T>(string value)
        {
            Type type = typeof(T);
            if (type.GetTypeInfo().IsEnum)
            {
                foreach (var name in Enum.GetNames(type))
                {
                    EnumMemberAttribute? attr = type.GetRuntimeField(name)
                        .GetCustomAttribute<EnumMemberAttribute>(true);
                    if (attr != null && attr.Value == value)
                    {
                        return (T) Enum.Parse(enumType: type, value: name);
                    }
                }

                return default;
            }

            throw new InvalidOperationException($"{value} is not a member of {type.Name}.");
        }
    }
}
