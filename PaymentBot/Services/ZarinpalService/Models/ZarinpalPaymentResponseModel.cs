using System.Collections.Generic;

namespace PaymentBot.Services.ZarinpalService.Models
{
    public class ZarinpalPaymentResponseModel
    {
        public string Authority { set; get; }

        public int Status { set; get; }

        public string PaymentUrl { set; get; }

        public string Message { get; set; }
    }

    internal static class ZarinpalPaymentResponseModelExtension
    {
        public static void Validate(this ZarinpalPaymentResponseModel model, List<ZarinpalError> errors)
        {
            if (model == null)
                errors.Add(new ZarinpalError { Code = "-2000", Description = $"Null reference exception. {nameof(model)}" });

            if (string.IsNullOrWhiteSpace(model?.Authority))
                errors.Add(new ZarinpalError { Code = "-2001", Description = "Authority cannot be null." });

            if (model?.Authority.Length != 36)
                errors.Add(new ZarinpalError { Code = "-2002", Description = "Authority is not valid." });

            //if (!long.TryParse(model?.Authority, out _))
            //    errors.Add(new ZarinpalError { Code = "-2003", Description = "Authority is not valid." });
        }
    }
}
