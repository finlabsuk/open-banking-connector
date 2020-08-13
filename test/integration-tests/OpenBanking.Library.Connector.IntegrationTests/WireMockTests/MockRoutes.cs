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
