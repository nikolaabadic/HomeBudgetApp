using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeBudgetApp.Domain;
using HomeBudgetApp.Data.UnitOfWork;
using HomeBudgetApp.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBudgetApp.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public ActionResult Index()
        {
            return View();
        }

        //Login
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            try
            {
                User user = unitOfWork.User.GetByUsernameAndPinCode(new User
                {
                    Username = model.Username,
                    Password = model.Password
                });
                HttpContext.Session.SetInt32("userid", user.UserID);
                return RedirectToAction("Details", "User");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, "Wrong credentials!");
                return View("Index");
            }
        }
    }
}
