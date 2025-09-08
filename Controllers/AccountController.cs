using Microsoft.AspNetCore.Mvc;

namespace part_1.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //register view
        public IActionResult Register()
        {
            return View();
        }

        //login view
        public IActionResult Login()
        {
            return View();
        }

        //forgot password view
        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}
