using MepasTask.Infrastructure.Repository;
using MepasTask.UI.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MepasTask.Model;
using MepasTask.App.Interfaces;

namespace MepasTask.UI.Controllers
{
    public class LoginController : Controller
    {
        //private IHttpContextAccessor _accessor;
        public LoginController()//IHttpContextAccessor accessor)//,IUnitOfWork unitOfWork)
        {
            //this.unitOfWork = unitOfWork;
           // _accessor = accessor;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(UsersModel loginModel)
        {

          HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("MepasTask");
            CookieHandler.TumCerezleriTemizle(HttpContext);

            try
            {

                var qs = new Dictionary<string, string>()
                {
                    ["username"] = loginModel.username.ToString(),
                    ["password"] = loginModel.password.ToString()
                };

                var model = API<UsersModel>.SelectPostAsyncFirstOrDefault("Login", "Check", qs).Result;

                if (model.username ==null)
                {
                    return Json(new { success = 0, message = "Lütfen bilgilerinizi kontrol ediniz" });
                }

                var claims = new List<Claim>
                {
                      new Claim("id", model.id.ToString()),
                      new Claim("username", model.username.ToString()),
                      new Claim(ClaimTypes.Name, model.name.ToString()),
                      new Claim(ClaimTypes.Surname, model.surname.ToString()),
                };

                var claimsIdentity = new ClaimsIdentity(
                          claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                };


                if (model.username != null)
                {

                    return RedirectToAction("Index", "Products");
                }
                else return RedirectToAction("Index", "Error");

            }
            catch (Exception ex)
            {
                return View("Error");
            }

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public async Task<IActionResult> Logout()
        {
            //await _accessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("BulutAidat");
            CookieHandler.TumCerezleriTemizle(HttpContext);
            return RedirectToAction("Index", "Login");
        }

    }
}
