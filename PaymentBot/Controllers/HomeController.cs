using Microsoft.AspNetCore.Mvc;

namespace PaymentBot.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult index()
        {
            return View();
        }

    }
}
