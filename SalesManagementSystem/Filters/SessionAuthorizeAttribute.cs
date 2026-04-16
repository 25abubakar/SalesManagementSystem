//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;

//namespace SalesManagementSystem.Filters
//{
//    public class SessionAuthorizeAttribute : ActionFilterAttribute
//    {
//        public override void OnActionExecuting(ActionExecutingContext context)
//        {
//            var user = context.HttpContext.Session.GetString("User");

//            if (string.IsNullOrEmpty(user))
//            {
//                context.Result = new RedirectToActionResult("Login", "Auth", null);
//            }
//        }
//    }
//}