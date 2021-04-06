﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http
{
    public class HttpRequestInfo
    {
        public HttpRequestInfo()
        {
            Method = HttpMethod.Get.ToString();
            MaxRedirects = 50;
            UserAgent = "OpenBankingConnector";
        }

        public Uri RequestUri { get; set; } = null!;
        public bool? UseDefaultCredentials { get; set; }
        public ICredentials? Credentials { get; set; }
        public bool? PreAuthenticate { get; set; }

        public string Method { get; set; }
        public List<HttpHeader> Headers { get; set; } = new List<HttpHeader>();
        public List<Cookie> Cookies { get; set; } = new List<Cookie>();
        public string UserAgent { get; set; }
        public TimeSpan Timeout { get; set; }
        public TimeSpan ReadWriteTimeout { get; set; }
        public int MaxRedirects { get; set; }
        public List<X509Certificate> Certificates { get; set; } = new List<X509Certificate>();
        public IWebProxy? Proxy { get; set; }

        public string? Authorisation { get; set; }
        public string? BearerToken { get; set; }

        public List<string> ContentTypes { get; } = new List<string>();

        public string Content { get; set; } = null!;
        public IServerCertificateValidator? ServerCertificateValidator { get; set; }
    }
}
