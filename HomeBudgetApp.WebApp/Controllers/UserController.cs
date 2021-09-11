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
    public class UserController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public UserController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: UserController
        public ActionResult Index()
        {
            return View("Login");
        }
        [LoggedInUser]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (!ModelState.IsValid)
            {
                if (user.Name == null || user.Surname == null || user.Username == null || user.Password == null)
                {
                    return View();
                }
            }

            User userDB = unitOfWork.User.Search(u => u.Username == user.Username);
            if (userDB == null)
            {
                unitOfWork.User.Add(user);
                unitOfWork.Commit();
                return RedirectToAction("Index", "Home");
            } else
            {
                ModelState.AddModelError(string.Empty, "Username is already taken!");
                return View(user);
            }
        }

        [LoggedInUser]
        public ActionResult ShowDetails(int id)
        {
            HttpContext.Session.SetInt32("userid", id);
            return RedirectToAction("Details");
        }

        // GET: UserController/Details/5
        [HttpGet]
        [LoggedInUser]
        public ActionResult Details()
        {
            int? id = HttpContext.Session.GetInt32("userid");
            User user = null;
            UserDetailsModel model = new UserDetailsModel();
            if (id != null)
            {
                user = unitOfWork.User.FindByID((int)id);
            }
            if (user != null)
            {
                model.UserID = user.UserID;
                model.Name = user.Name;
                model.Surname = user.Surname;

                byte[] accountsByte = HttpContext.Session.Get("templates");
                if (accountsByte == null)
                {
                    model.Accounts = unitOfWork.Account.Search(a => a.UserID == user.UserID && !a.Hidden);
                }
                else
                {
                    model.Accounts = System.Text.Json.JsonSerializer.Deserialize<List<Account>>(accountsByte);
                }
                
            }
            return View(model);
        }

        [HttpGet]
        [LoggedInUser]
        public ActionResult Search(List<User> model)
        {
            byte[] usersByte = HttpContext.Session.Get("users");
            if (usersByte == null)
            {
                return View(model);
            }
            List<User> users = System.Text.Json.JsonSerializer.Deserialize<List<User>>(usersByte);
            return View(users);

        }
        [HttpGet]
        [LoggedInUser]
        public ActionResult GetUsersByUsername(string Username)
        {
            List<User> users = unitOfWork.User.SearchUsers(u => 
                u.Username.ToLower().Contains(Username.ToLower()) || 
                u.Name.ToLower().Contains(Username.ToLower()) ||
                u.Surname.ToLower().Contains(Username.ToLower()));

            HttpContext.Session.Set("users", System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(users));

            return Json(new { redirectToUrl = Url.Action("Search") });
        }
    }
}
