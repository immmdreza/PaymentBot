namespace PaymentBot.Services.ZarinpalService
{
    internal static class ZarinpalUrlConfig
    {
        private const string RequestUrl = "https://{0}zarinpal.com/pg/rest/WebGate/PaymentRequest.json";

        private const string PgUrl = "https://{0}zarinpal.com/pg/StartPay/{1}";
        private const string WebGateUrl = "https://{0}zarinpal.com/pg/StartPay/{1}/ZarinGate";

        private const string VerificationUrl = "https://{0}zarinpal.com/pg/rest/WebGate/PaymentVerification.json";
        private const string SandBoxPerfix = "sandbox.";
        private const string WwwPerfix = "www.";

        public static string GetPaymentRequestUrl(bool useSanbox)
        {
            return string.Format(RequestUrl, useSanbox ? SandBoxPerfix : WwwPerfix);
        }

        public static string GetPaymenGatewayUrl(string authority, bool useSanbox)
        {
            return string.Format(PgUrl, useSanbox ? SandBoxPerfix : WwwPerfix, authority);
        }

        public static string GetVerificationUrl(bool useSanbox)
        {
            return string.Format(VerificationUrl, useSanbox ? SandBoxPerfix : WwwPerfix);
        }

        public static string GetWebGateRequestUrl(string authority, bool useSanbox)
        {
            return string.Format(WebGateUrl, useSanbox ? SandBoxPerfix : WwwPerfix, authority);
        }
    }
}
