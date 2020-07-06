using Microsoft.AspNetCore.Mvc;
using PaymentBot.Data;
using PaymentBot.Models;
using PaymentBot.Services.ZarinpalService;
using PaymentBot.Services.ZarinpalService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PaymentBot.Controllers
{
    [Route("api/[controller]")]
    [RequireHttps]
    public class UpdateController : Controller
    {
        private static readonly TelegramBotClient botClient = new TelegramBotClient("botToken");
        private static readonly List<int> DonateStat = new List<int>();
        private const string CancellationText = "بیخیال";

        private readonly IZarinpalProvider _zarinpal;
        private readonly PaymentContext _paymentContext;

        public UpdateController(IZarinpalProvider zarinpal,
            PaymentContext paymentContext)
        {
            _zarinpal = zarinpal;
            _paymentContext = paymentContext;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            if (update is null) { return BadRequest(); }

            switch (update)
            {
                case { Message: { Text: { } text } message }:
                    {
                        string[] Splited = text.ToLower().Split(' ');

                        if (DonateStat.Any(x => x == message.From.Id))
                        {
                            if (text == CancellationText)
                            {
                                await botClient.SendTextMessageAsync(message.Chat.Id,
                                    "باشه عزیزم، اگه کار دیگه ای داشتی خبرم کن.\n\nاگر می خواهی دوباره امتحان کنی دستور /pay رو ارسال کن."
                                    , replyMarkup: new ReplyKeyboardRemove());

                                DonateStat.Remove(message.From.Id);

                                return Ok();
                            }

                            if (!int.TryParse(text, out int Amount))
                            {
                                await botClient.SendTextMessageAsync(message.Chat.Id,
                                    $"لطفا مقداری عددی بین 1000 تا 500000 رو ارسال کن. اگه منصرف شدی از دکمه {CancellationText} استفاده کن.");

                                return Ok();

                            }

                            if (Amount < 1000 || Amount > 500000)
                            {
                                await botClient.SendTextMessageAsync(message.Chat.Id,
                                    $"لطفا مقداری عددی بین 1000 تا 500000 رو ارسال کن. اگه منصرف شدی از دکمه {CancellationText} استفاده کن.");

                                return Ok();

                            }

                            DonateStat.Remove(message.From.Id);

                            await botClient.SendTextMessageAsync(message.Chat.Id,
                                $"خب طبق گفته خودت می خوای {Amount} نومان به ما حمایت مال ارسال کنی. آیا از تصمیمت مطمعنی؟!",
                                replyMarkup: new InlineKeyboardMarkup(
                                    new InlineKeyboardButton[][]
                                    {
                                                new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("صد در صد مطمعنم",$"pay|{Amount}|0") },
                                                new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("میخوام مبلغ و تغییر بدم", "donate") },
                                                new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("یهو منصرف شدم :|","cancell") }
                                    }));

                            await botClient.SendTextMessageAsync(message.Chat.Id,
                                $"دکمه {CancellationText} و پاک کردم."
                                , replyMarkup: new ReplyKeyboardRemove());

                            return Ok();

                        }

                        switch (Splited[0])
                        {

                            case "/start":
                                {
                                    await botClient.SendTextMessageAsync(message.Chat.Id,
                                        $"سلام {TextHtmlParser(message.From.FirstName, ParseType.TextMention, message.From.Id.ToString())}"
                                        + $" به کمک من می تونی پرداخت های مرتبط با {TextHtmlParser("Task System", ParseType.Bold)} رو به راحتی انجام بدی."
                                        + "\n\nبرای شروع پرداخت و انتخاب نوع اون از دستور /pay استفاده کن.\n\nهمچنین با استفاده از دستورات /help  و /about، می تونی راهنمایی و اطلاعات بیشتری درباره ربات کسب کنی.",
                                        ParseMode.Html);

                                    return Ok();
                                }

                            case "/pay":
                                {
                                    await botClient.SendTextMessageAsync(message.Chat.Id,
                                        $"خب {message.From.FirstName}, خوشحالم که قراره از طرف تو یه پولی به دستمون برسه. با استفاده از دکمه های زیر بهم بگو دقیقا هدفت از این پرداخت چیه؟!",
                                        replyMarkup: new InlineKeyboardMarkup(
                                            new InlineKeyboardButton[][]
                                            {
                                                new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("میخوام حمایت مالی کنم","donate")},
                                                new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("میخواستم تبلیغ رزرو کنم","adsorder")},
                                                new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("قرار نیس پولی بدم خوشحال نباش","cancell")}
                                            }));

                                    return Ok();

                                }

                            case "/about":
                                {
                                    await botClient.SendTextMessageAsync(message.Chat.Id,
                                        $"من رباتی تقریبا ساده برای انجام پرداخته های مربوط به {TextHtmlParser("Task System", ParseType.Bold)} هستم"
                                        + $" من با استفاده از زبان برنامه نویسی سی شارپ ({TextHtmlParser("C#", ParseType.MonoSpace)}) و تکنولوژی {TextHtmlParser("Asp .NET Core MVC", ParseType.MonoSpace)} توسعه و طراحل شدم."
                                        + $" و از روش {TextHtmlParser("Webhook", ParseType.MonoSpace)} برای دریافت آپدیت ها استفاده می کنم."
                                        + $"اگر اطلاعات بیشتری نیاز داری به {TextHtmlParser("گروه پشتیبانی", ParseType.HyperLink, "https://t.me/tsww_support")} مراجعه کن.",
                                        ParseMode.Html, true);

                                    return Ok();
                                }

                            case "/help":
                                {
                                    await botClient.SendTextMessageAsync(message.Chat.Id,
                                        $"راستش کار کردن با من خیلی راحته، ولی با این حال اگر به مشکلی برخوردی، میخوای مشکلی رو گزارش کنی یا اطلاعات بیشتری نیاز داری به {TextHtmlParser("گروه پشتیبانی", ParseType.HyperLink, "https://t.me/tsww_support")} مراجعه کن.",
                                        ParseMode.Html,
                                        replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("گروه پشتیبانی", "https://t.me/tsww_support")));

                                    return Ok();
                                }

                            default:
                                return Ok();
                        }
                    }

                case { CallbackQuery: { Data: { } data } callback }:
                    {
                        string[] Splited = data.Split('|');

                        switch (Splited[0])
                        {
                            case "adsorder":
                                {
                                    await botClient.EditMessageTextAsync(callback.Message.Chat.Id, callback.Message.MessageId,
                                        $"تبلیغ کردن خیلی روش باحالایه امیدوارم که بتونیم بیشترین بازده رو داشته باشیم. "
                                        + $"<b>حواست باشه قبل از پرداخت با ادمینای ربات مذاکره کرده باشی و از شرایط تبلیغات و شیوه انجامش اگاهی کامل داشته باشی</b>"
                                        + "\n\nاز بین گزینه های زیر یکی از پلن ها رو که مورد علاقته انتخاب کن."
                                        + $"\n\nاگه نیاز داری که با ادمین های ربات صحبت کنی:"
                                        + $"\n-{TextHtmlParser("گروه پشتیبانی", ParseType.HyperLink, "https://t.me/tsww_support")}"
                                        + $"\n-{TextHtmlParser("گروه تبلیغات", ParseType.HyperLink, "https://t.me/tsww_ads")}",
                                        ParseMode.Html, disableWebPagePreview: true,
                                        replyMarkup: new InlineKeyboardMarkup(
                                            new InlineKeyboardButton[][]
                                            {
                                                new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("یک روزه 2000 تومان","ap|1")},
                                                new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("سه روزه 5000 تومان", "ap|2") },
                                                new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("یک هفته 12000 تومان", "ap|3") },
                                                new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("15 روزه 26000 تومان", "ap|4") },
                                                new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("یک ماهه 50000 تومان", "ap|5") },
                                                new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("یهو منصرف شدم :|","cancell")}
                                            }));

                                    return Ok();
                                }

                            case "donate":
                                {
                                    await botClient.EditMessageTextAsync(callback.Message.Chat.Id, callback.Message.MessageId,
                                        $"ما عمیقا خیلی خوشحالیم که قراره در پیشرفت و توسعه ربات سهمی داشته باشید. "
                                        + $"<b>جهت حمایت مالی، مبلغ مورد نظرتونو به من ارسال کنید. این مبلغ می تونه بین 1000 تا 500000 تومن باشه.</b>"
                                        + $"\n\nاگه نیاز داری که با ادمین های ربات صحبت کنی:"
                                        + $"\n-{TextHtmlParser("گروه پشتیبانی", ParseType.HyperLink, "https://t.me/tsww_support")}"
                                        + $"\n-{TextHtmlParser("گروه تبلیغات", ParseType.HyperLink, "https://t.me/tsww_ads")}",
                                        parseMode: ParseMode.Html,
                                        disableWebPagePreview: true);

                                    await botClient.SendTextMessageAsync(callback.Message.Chat.Id,
                                        "خب مبلغ مورد نظرتو بفرست ببینم چکار می کنی، اگه خدایی نکرده منصرف شدی از دکمه بیخیال استفاده کن.",
                                        replyMarkup: new ReplyKeyboardMarkup(new KeyboardButton[] { new KeyboardButton(CancellationText) },
                                        resizeKeyboard: true));

                                    DonateStat.Add(callback.From.Id);

                                    return Ok();
                                }

                            case "ap":
                                {
                                    string plan = Splited[1];
                                    int Amount = GetPlanAmount(plan);
                                    if (Amount == 0)
                                    {
                                        await botClient.EditMessageTextAsync(callback.Message.Chat.Id, callback.Message.MessageId,
                                            "پلن مورد نظر معتبر نیست لطفا دوباره امتحان کنید.");
                                    }

                                    await botClient.EditMessageTextAsync(callback.Message.Chat.Id, callback.Message.MessageId,
                                        $"مثل اینکه پلن {plan} به مبلغ {Amount} رو انتخاب کردی، اگه واسه پرداخت آماده ای بهم بگو.",
                                        replyMarkup: new InlineKeyboardMarkup(
                                            new InlineKeyboardButton[][]
                                            {
                                                new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("صد در صد مطمعنم",$"pay|{Amount}|1") },
                                                new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("میخوام پلن و تغییر بدم", "adsorder") },
                                                new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("یهو منصرف شدم :|","cancell") }
                                            }));

                                    return Ok();
                                }

                            case "pay":
                                {
                                    var msg = await botClient.EditMessageTextAsync(callback.Message.Chat.Id, callback.Message.MessageId,
                                        "چند لحظه ...");

                                    int amount = int.Parse(Splited[1]);
                                    PaymentGoal type = (PaymentGoal)int.Parse(Splited[2]);
                                    string description = type == PaymentGoal.Ads ? $"رزرو تبلیغات به مبلغ {amount}" :
                                        $"حمایت مالی از ربات به مبلغ {amount}";

                                    ZarinpalPaymentRequestModel model = new ZarinpalPaymentRequestModel
                                    {
                                        Amount = amount,
                                        Description = description,
                                        CallbackUrl = $"https://yourHost/Verify/Index",
                                        Email = "",
                                        Mobile = callback.From.Id.ToString()

                                    };

                                    ZarinpalResult<ZarinpalPaymentResponseModel> t = await _zarinpal.PayAsync(model);

                                    if (t.Succeeded && t.Result.Status == 100)
                                    {
                                        PaymentGoal taget = type;

                                        _paymentContext.TsPayments.Add(new Payment
                                        {
                                            Authority = t.Result.Authority,
                                            Amount = amount,
                                            Description = description,
                                            Email = "",
                                            Mobile = callback.From.Id.ToString(),
                                            IsSuccesseded = false,
                                            IsCompleted = false,
                                            PaymentGoal = taget,
                                            PayMessageId = msg.MessageId
                                        });

                                        await _paymentContext.SaveChangesAsync();

                                        await botClient.EditMessageTextAsync(callback.Message.Chat.Id, callback.Message.MessageId,
                                            "خیلی هم خوب لینک پرداخت شما آماده شد. با استفاده از دکمه زیر می تونی به صفحه پرداخت بری و پرداخت و انجام بدی",
                                            replyMarkup: new InlineKeyboardMarkup(
                                                InlineKeyboardButton.WithUrl("بریم به درگاه", t.Result.PaymentUrl)));
                                        return Ok();
                                    }
                                    else
                                    {
                                        await botClient.EditMessageTextAsync(callback.Message.Chat.Id, callback.Message.MessageId,
                                            "تولید لینک پرداخت موفق نبود لطفا دوباره امتحان کنید"
                                            + $"\n\nخطا: {string.Join('\n', t.Errors.Select(x => x.Description))}");
                                        return Ok();
                                    }
                                }

                            case "cancell":
                                {
                                    await botClient.EditMessageTextAsync(callback.Message.Chat.Id, callback.Message.MessageId,
                                        "باشه عزیزم، اگه کار دیگه ای داشتی خبرم کن.\n\nاگر می خواهی دوباره امتحان کنی دستور /pay رو ارسال کن.");

                                    return Ok();
                                }

                            default:
                                return Ok();
                        }
                    }

                default: return Ok();
            }
        }

        private static int GetPlanAmount(string planid)
        {
            return planid switch
            {
                "1" => 2000,
                "2" => 5000,
                "3" => 12000,
                "4" => 26000,
                "5" => 50000,
                _ => 0,
            };
        }

        private static string TextHtmlParser(string firstText, ParseType parseType, string secondObj = "")
        {
            switch (parseType)
            {
                case ParseType.HyperLink:
                    {
                        return $"<a href='{secondObj}'>{HtmlEncoder.Default.Encode(firstText)}</a>";
                    }
                case ParseType.Bold:
                    {
                        return $"<b>{HtmlEncoder.Default.Encode(firstText)}</b>";
                    }
                case ParseType.MonoSpace:
                    {
                        return $"<code>{HtmlEncoder.Default.Encode(firstText)}</code>";
                    }
                case ParseType.TextMention:
                    {
                        return $"<a href='tg://user?id={secondObj}'>{HtmlEncoder.Default.Encode(firstText)}</a>";
                    }
                default:
                    return string.Empty;
            }
        }

        private enum ParseType
        {
            HyperLink,
            Bold,
            MonoSpace,
            TextMention
        }
    }
}
