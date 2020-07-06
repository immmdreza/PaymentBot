using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentBot.Data;
using PaymentBot.Models;
using PaymentBot.Services.ZarinpalService;
using PaymentBot.Services.ZarinpalService.Models;
using System;
using System.Threading.Tasks;
using Telegram.Bot;

namespace PaymentBot.Controllers
{
    [RequireHttps]
    public class VerifyController : Controller
    {
        private static readonly TelegramBotClient botClient = new TelegramBotClient("1074877476:AAH83JPEgRH-qzpx9qzplEfBhU_9PbLmYiE");
        private readonly IZarinpalProvider _zarinpal;
        private readonly PaymentContext _paymentContext;

        public VerifyController(IZarinpalProvider zarinpal,
            PaymentContext paymentContext)
        {
            _zarinpal = zarinpal;
            _paymentContext = paymentContext;
        }

        public async Task<IActionResult> Index(string authority, string status)
        {
            try
            {
                Payment payment = await _paymentContext.TsPayments.FirstOrDefaultAsync(x => x.Authority == authority);
                int uid = string.IsNullOrEmpty(payment.Mobile) ? 0 : int.Parse(payment.Mobile);

                if (payment is null)
                {
                    return View(new PayResult { success = false, Result = "تراکنش مورد نظر یافت نشد" });
                }

                if (status == "OK")
                {
                    if (payment.IsCompleted)
                    {
                        return View(new PayResult { success = false, Result = "این تراکنش قبلا کامل شده است." });
                    }

                    ZarinpalResult<ZarinpalVerificationResponseModel> result = await _zarinpal.VerifyAsync(new ZarinpalPaymentVerificationModel(payment.Amount, authority));

                    if (result.Result.Status == 100)
                    {
                        string resultMessage = "";

                        if (payment.PaymentGoal == PaymentGoal.Ads)
                        {
                            resultMessage = "پرداخت شما با موفقیت انجام شده. برای فعال سازی تبلیغ خود به ادمین ربات مراجعه کنید.";

                        }
                        else if (payment.PaymentGoal == PaymentGoal.Donate)
                        {
                            resultMessage = "با تشکر فراوان از حمایت مال شما از پروژه ربات، پرداخت با موفقیت انجام شد ❤";
                        }

                        payment.Description += ", پرداخت موفق با شماره پیگیری: " + result.Result.ReferenceId;
                        payment.IsSuccesseded = true;
                        await _paymentContext.SaveChangesAsync();

                        await botClient.SendTextMessageAsync(uid, "تراکنش موفق\n\n" + resultMessage + "\n\nشماره پیگیری: " + result.Result.ReferenceId);

                        try
                        {
                            await botClient.DeleteMessageAsync(uid, payment.PayMessageId);
                        }
                        catch { }

                        return View(new PayResult { success = true, referenceId = result.Result.ReferenceId, Result = resultMessage });
                    }
                    else
                    {
                        payment.Description += ", تراکنش ناموفق شماره پیگیری: " + result.Result.ReferenceId;

                        payment.IsCompleted = true;
                        await _paymentContext.SaveChangesAsync();

                        await botClient.SendTextMessageAsync(uid, payment.Description);

                        try
                        {
                            await botClient.DeleteMessageAsync(uid, payment.PayMessageId);
                        }
                        catch { }

                        return View(
                            new PayResult { success = false, referenceId = result.Result.ReferenceId, Result = GetMessage(result.Result.Status) });
                    }
                }
                else
                {
                    payment.Description += "، تراکنش ناموفق کنسل شده توسط کاربر";

                    payment.IsCompleted = true;
                    await _paymentContext.SaveChangesAsync();

                    await botClient.SendTextMessageAsync(uid, payment.Description);

                    try
                    {
                        await botClient.DeleteMessageAsync(uid, payment.PayMessageId);
                    }
                    catch { }

                    return View(new PayResult { success = false, Result = "تراکنش توسط کاربر کنسل شد." });
                }
            }
            catch(Exception e)
            {
                return View(new PayResult { success = false, Result = e.Message });
            }
        }

        public string GetMessage(int status)
        {
            return status switch
            {
                -1 => "اطلاعات ارسال شده  ناقص است.",
                -2 => "IP و یا مرچنت کد پذیرنده صحیح نیست.",
                -3 => "با توجه به محدودیت های شاپرک امکان پرداخت با رقم درخواست شده میسر نمی باشد.",
                -4 => "سطح تایید پذیرنده پایین تر از سطح نقره ای است.",
                -11 => "درخواست مورد نظر یافت نشد.",
                -12 => "امکان ویرایش درخواست میسر نمی باشد.",
                -21 => "هیچ نوع عملیات مالی برای این تراکنش یافت نشد.",
                -22 => "تراکنش ناموفق می باشد.",
                -33 => "رقم تراکنش با رقم پرداخت شده مطابقت ندارد.",
                -34 => "سقف تقسیم تراکنش از لحاظ تعداد یا رقم عبور نموده است.",
                -40 => "اجازه دسترسی به متد مربوطه وجود ندارد.",
                -41 => "اطلاعات ارسال شده مربوط به AdditionalData غیر معتبر می باشد.",
                -42 => "مدت زمان معتبر طول عمر شناسه پرداخت باید بین 30 دقیقه تا 45 روز می باشد.",
                -54 => "درخواست مورد نظر آرشیو شده است.",
                100 => "عملیات با موفقیت انجام گردیده است.",
                101 => "عملیات پرداخت موفق بوده و قبلاً PaymentVerification تراکنش انجام شده است.",
                _ => "کد تعریف نشده.",
            };
        }

    }
}