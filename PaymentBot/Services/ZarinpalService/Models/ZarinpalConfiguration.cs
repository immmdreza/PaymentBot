using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentBot.Services.ZarinpalService.Models
{
    public class ZarinpalConfiguration
    {
        public string Token { get; set; }

        public bool UseSandbox { get; set; }

        public bool UseZarinLink { get; set; }
    }
}
