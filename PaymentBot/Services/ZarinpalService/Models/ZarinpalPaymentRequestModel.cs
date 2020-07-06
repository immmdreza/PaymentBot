using Newtonsoft.Json;
using System.Collections.Generic;

namespace PaymentBot.Services.ZarinpalService.Models
{
    public class ZarinpalPaymentRequestModel
    {
        [JsonProperty("MerchantID")]
        public string MerchantId { get; set; }

        [JsonProperty("CallbackURL")]
        public string CallbackUrl { get; set; }

        public string Description { get; set; }

        public long Amount { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public ZarinpalPaymentRequestModel()
        {

        }

        public ZarinpalPaymentRequestModel(long amount, string callbackUrl, string description)
        {
            Amount = amount;
            CallbackUrl = callbackUrl;
            Description = description;
        }

        public ZarinpalPaymentRequestModel(long amount, string callbackUrl, string description, string mobile, string email)
        {
            Amount = amount;
            CallbackUrl = callbackUrl;
            Description = description;
            Mobile = mobile;
            Email = email;
        }
    }

    internal static class ZarinpalPaymentRequestModelExtension
    {
        public static void ValidateModel(this ZarinpalPaymentRequestModel model, List<ZarinpalError> errors)
        {
            if (model == null)
                errors.Add(new ZarinpalError());

            if (string.IsNullOrWhiteSpace(model?.CallbackUrl))
                errors.Add(new ZarinpalError { Code = "-1001", Description = $"Value cannot be null: {nameof(model.CallbackUrl)}." });

            if (string.IsNullOrWhiteSpace(model?.Description))
                errors.Add(new ZarinpalError { Code = "-1002", Description = $"Value cannot be null: {nameof(model.Description)}." });

            if (model?.Amount < 1000)
                errors.Add(new ZarinpalError { Code = "-1003", Description = $"{nameof(model.Amount)} cannot be less than 1000." });

            //if (string.IsNullOrWhiteSpace(model?.MerchantId))
            //    errors.Add(new ZarinpalError { Code = "-1004", Description = $"Authorization token is not present: {nameof(model.MerchantId)}" });
        }
    }
}
