using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeBudgetApp.Domain;
using HomeBudgetApp.Data.UnitOfWork;
using HomeBudgetApp.WebApp.Filters;
using HomeBudgetApp.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBudgetApp.WebApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminUnitOfWork unitOfWork;

        public AdminController(IAdminUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: AdminController
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
                Admin admin = unitOfWork.Admin.GetByUsernameAndPinCode(new Admin { Username = model.Username, Password = model.Password });
                HttpContext.Session.SetInt32("adminid", admin.AdminID);
                return View("Options");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, "Wrong credentials!");
                return View("Index");
            }
        }
        [AdminLoggedIn]
        public ActionResult Options()
        {
            return View("Options");
        }
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Admin");
        }
        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
    }
}
