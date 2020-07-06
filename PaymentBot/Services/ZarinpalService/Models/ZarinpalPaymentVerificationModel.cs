using Newtonsoft.Json;
using System.Collections.Generic;

namespace PaymentBot.Services.ZarinpalService.Models
{
    public class ZarinpalPaymentVerificationModel
    {
        public long Amount { get; }

        [JsonProperty("MerchantID")]
        public string MerchantId { get; }

        public string Authority { get; }

        public ZarinpalPaymentVerificationModel() { }

        internal ZarinpalPaymentVerificationModel(string token, long amount, string authority)
        {
            MerchantId = token;
            Amount = amount;
            Authority = authority;
        }

        public ZarinpalPaymentVerificationModel(long amount, string authority)
        {
            Amount = amount;
            Authority = authority;
        }
    }

    internal static class ZarinpalPaymentVerificationModelExtension
    {
        public static void ValidateModel(this ZarinpalPaymentVerificationModel model, List<ZarinpalError> errors)
        {
            if (model == null)
                errors.Add(new ZarinpalError { Code = "-3000", Description = $"Null reference exception. {nameof(model)}" });

            if (string.IsNullOrWhiteSpace(model?.Authority))
                errors.Add(new ZarinpalError { Code = "-3001", Description = "Authority cannot be null." });

            if (model?.Authority.Length != 36)
                errors.Add(new ZarinpalError { Code = "-3002", Description = "Authority is not valid." });

            //if (!long.TryParse(model?.Authority, out _))
            //    errors.Add(new ZarinpalError { Code = "-3003", Description = "Authority is not valid." });

            if (model?.Amount < 1000)
                errors.Add(new ZarinpalError { Code = "-3004", Description = $"{nameof(model.Amount)} cannot be less than 1000." });

            //if (string.IsNullOrWhiteSpace(model?.MerchantId))
            //    errors.Add(new ZarinpalError { Code = "-3005", Description = $"Authorization token is not present: {nameof(model.MerchantId)}" });
        }
    }
}
