using Microsoft.AspNetCore.Mvc;

namespace PeachMvc.Controllers
{
    public class PaymentFormController : Controller
    {
        public IActionResult PaymentFormView()
        {
            return View();
        }
    }
}
