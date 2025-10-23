using Microsoft.AspNetCore.Mvc;
using part_1.Models;

namespace part_1.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //register view
        [HttpGet]
        public IActionResult register()
        {
            ViewBag.id = HttpContext.Session.GetString("id");
            ViewBag.role = HttpContext.Session.GetString("role");
            return View();
        }

        [HttpPost]
        public IActionResult register(register_users user)
        {
            //  ViewBag.id = HttpContext.Session.GetString("id");
            //ViewBag.role = HttpContext.Session.GetString("role");

            Console.WriteLine(user.role);
            Console.WriteLine(user.email);

            Console.WriteLine(user.password);

            Console.WriteLine(user.ConfirmPassword);
            Console.WriteLine(user.name);
            Console.WriteLine(user.gender);

            if (ModelState.IsValid)
            {
                all_method method = new all_method();
                string result = method.register_user(user);

                if (result == "success")
                {
                    ViewBag.message = "User registered successfully!";

                  
                    return RedirectToAction("Login","Account");
                }
                else
                {
                    ViewBag.message = "Registration failed.";
                }
            }

            return View(user);
        }



        //login view
        [HttpGet]
        public IActionResult Login()
        {
            auto_create_check check = new auto_create_check();
            check.InitializeSystem();



            return View();
        }

        [HttpPost]
        public IActionResult Login(login user)
        {


            if (ModelState.IsValid)
            {
                all_method method = new all_method();
                string gets = method.login_user(user.email, user.role, user.password);

                if (gets.Contains(","))
                {
                    string[] found = gets.Split(",");

                    HttpContext.Session.SetString("id", found[0]);
                    HttpContext.Session.SetString("role", found[1]);

                    if (found[1]=="lecturer")
                    {
                        return RedirectToAction("Index", "Lecture");

                    }
                    else if (found[1] == "programme coordinator")
                    {
                        return View("/Views/ProgramCoordinator/Index.cshtml");
                    }
                    else if (found[1] == "programme manager")
                    {
                        return View("/Views/ProgramManager/Index.cshtml");
                    }
                }
                else
                {
                    ViewBag.message = "Incorrect username [Email ],role or password..";

                }

            }
            else
            {
                ViewBag.message = "All fields are required!!";
            }
            return View(user);
        }

        //forgot password view
        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}
