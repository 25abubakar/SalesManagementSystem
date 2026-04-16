//using Microsoft.AspNetCore.Mvc;
//using SalesManagementSystem.Models.Auth;

//namespace SalesManagementSystem.Controllers
//{
//    public class AuthController : Controller
//    {
//        public IActionResult Login()
//        {
//            return View();
//        }

//        [HttpPost]
//        public IActionResult Login(LoginViewModel model)
//        {
//            if (!ModelState.IsValid)
//                return View(model);

//            if (model.Username == "laltech" && model.Password == "lal@123")
//            {
//                HttpContext.Session.SetString("User", model.Username);
//                return RedirectToAction("Index", "Home");
//            }

//            model.ErrorMessage = "Invalid username or password";
//            return View(model);
//        }

//        public IActionResult Logout()
//        {
//            HttpContext.Session.Clear();
//            return RedirectToAction("Login");
//        }
//    }
//}