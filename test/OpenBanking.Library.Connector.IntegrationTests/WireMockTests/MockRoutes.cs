// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.WireMockTests
{
    public static class MockRoutes
    {
        public static string Url = "http://localhost:8080";
        public static string OpenId = "/.well-known/openid-configuration";
        public static string Register = "/register";
        public static string Token = "/token";
        public static string Authorize = "/authorize";
        public static string DomesticPaymentConsents = "/domestic-payment-consents";
        public static string DomesticPayments = "/domestic-payments";
    }
}
