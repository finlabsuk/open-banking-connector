// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using SoftwareStatementProfileCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Repository.SoftwareStatementProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.Repositories
{
    public class SoftwareStatementProfileCache : MemoryRepository<SoftwareStatementProfileCached>
    {
        public SoftwareStatementProfileCache(
            ISettingsProvider<OpenBankingConnectorSettings> obcSettingsProvider,
            ISettingsProvider<SoftwareStatementProfilesSettings> softwareStatementProfilesSettingsProvider,
            IInstrumentationClient instrumentationClient)
        {
            OpenBankingConnectorSettings obcSettings = obcSettingsProvider.GetSettings();
            obcSettings.ArgNotNull(nameof(obcSettings));

            SoftwareStatementProfilesSettings softwareStatementProfilesSettings =
                softwareStatementProfilesSettingsProvider.GetSettings();
            softwareStatementProfilesSettings.ArgNotNull(nameof(softwareStatementProfilesSettings));

            List<string> activeProfileIds = obcSettings.ProcessedSoftwareStatementProfileIds.ToList();
            foreach (string id in activeProfileIds)
            {
                bool success = softwareStatementProfilesSettings.TryGetValue(id, out SoftwareStatementProfile profile);
                if (success == false)
                {
                    throw new ArgumentOutOfRangeException(id);
                }

                // TODO: validate profile
                List<X509Certificate2> transportCerts = new List<X509Certificate2>();
                if (profile.CertificateType == CertificateType.LegacyOB.ToString())
                {
                    X509Certificate2 transportCert =
                        CertificateFactories.GetCertificate2FromPem(
                            profile.TransportKey!,
                            profile.TransportCertificate!) ??
                        throw new InvalidOperationException();
                    transportCerts.Add(transportCert);
                }

                HttpMessageHandler handler = new HttpRequestBuilder()
                    .SetClientCertificates(transportCerts)
                    .CreateMessageHandler();

                SoftwareStatementProfileCached x = new SoftwareStatementProfileCached(
                    id,
                    profile,
                    new ApiClient(instrumentationClient, new HttpClient(handler)));

                _cache.TryAdd(id, x);
            }
        }
    }
}
