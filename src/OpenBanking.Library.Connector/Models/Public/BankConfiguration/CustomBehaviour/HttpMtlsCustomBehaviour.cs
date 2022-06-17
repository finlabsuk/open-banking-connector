// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;

    public class HttpMtlsCustomBehaviour
    {       
        /// <summary>
        ///     Disable verification of external bank TLS certificates when using mutual TLS with this certificate profile. Not
        ///     intended for
        ///     production use but
        ///     sometimes helpful for diagnosing issues with bank sandboxes (e.g. if they use self-signed certificates).
        /// </summary>
        public bool? DisableTlsCertificateVerification { get; set; }

        //public string? TlsRenegotiationSupport { get; set; }
    }
