# PaymentBot ![.NET Core](https://github.com/immmdreza/PaymentBot/workflows/.NET%20Core/badge.svg)
Telegram bot for payments using Zarinpal gateway, and Webhooks

## Setup
1. Be aware of installed packages :
   * [Telagram.Bot](https://github.com/TelegramBots/telegram.bot)
   * [Microsoft.EntityFrameworkCore.SqlServer](https://docs.microsoft.com/en-gb/ef/core/)
   * [Microsoft.EntityFrameworkCore.Tools](https://docs.microsoft.com/en-gb/ef/core/) (required only if you gonna setup and update database using *Migration Tools*)
   * [Microsoft.AspNetCore.Mvc.NewtonsoftJson](https://www.asp.net/web-api)
   * [Microsoft.AspNet.WebApi.Client](https://www.asp.net/web-api)
   
2. Edit *appsettings.json* and with your own connectionString and ZarinpalToken **(yourMerchantID)**

3. You should also replace your own botToken in Verify and Update Controller

4. Setup webhook for your telegram bot to UpdateController

5. It's probably ready for publish.

