using Newtonsoft.Json;
using System;

namespace PaymentBot.Services.ZarinpalService.Models
{
    public class ZarinpalVerificationResponseModel
    {
        [JsonProperty("RefId")]
        public string ReferenceId { get; set; }

        public int Status { get; set; }

        public override string ToString()
        {
            return Status.MessageToString();
        }
    }

    internal static class StatusMessage
    {
        public static string MessageToString(this int status)
        {
            switch (status)
            {
                case -1:
                    return "اطلاعات ارسال شده ناقص است.";
                case -2:
                    return "اطلاعات پذیرنده معتبر نیستند!";
                case -3:
                    return "با توجه به محدودیت‌های شاپرک امکان پرداخت با رقم درخواست شده میسر نمی‌باشد.";
                case -4:
                    return "سطح تایید پذیرنده پایین‌تر از سطح نقره‌ای است.";
                case -11:
                    return "درخواست مورد نظر یافت نشد!";
                case -12:
                    return "امکان ویرایش درخواست میسر نمی‌باشد!";
                case -21:
                    return "هیچ نوع عملیات مالی برای این تراکنش یافت نشد.";
                case -22:
                    return "تراکنش ناموفق است!";
                case -33:
                    return "رقم تراکنش با رقم پرداخت شده مطابقت ندارد.";
                case -34:
                    return "سقف تقسیم تراکنش از لحاظ تعداد یا رقم عبور نموده است.";
                case -40:
                    return "اجازه دسترسی به متد مربوطه وجود ندارد.";
                case -41:
                    return "اطلاعات ارسال شده مربوط به داده‌های اضافی، غیرمعتبر می‌باشد.";
                case -42:
                    return "مدت زمان معتبر طول عمر شناسه پرداخت باید بین ۳۰ دقیقه تا ۴۵ روز باشد.";
                case -54:
                    return "درخواست مورد نظر بایگانی شده است.";
                case -1000:
                    return "تراکنش توسط کاربر لغو شد!";
                case 100:
                    return "عملیات با موفقیت انجام گردیده است.";
                case 101:
                    return "عملیات پرداخت موفق بوده و پیشتر تصدیق پرداخت تراکنش انجام شده است.";
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }
    }
}
