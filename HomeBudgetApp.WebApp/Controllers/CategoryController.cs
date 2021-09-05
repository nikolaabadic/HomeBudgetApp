using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeBudgetApp.Data.UnitOfWork;
using HomeBudgetApp.Domain;
using HomeBudgetApp.WebApp.Filters;
using Microsoft.AspNetCore.Mvc;

namespace HomeBudgetApp.WebApp.Controllers
{
    [AdminLoggedIn]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        // GET: PaymentCardController/Create
        public ActionResult Create(int id)
        {
            return View("Create");
        }

        // POST: PaymentCardController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            try
            {
                unitOfWork.Category.Add(category);
                unitOfWork.Commit();
                return RedirectToAction("Index");
            }
            catch
            {
                return View("Create", category);
            }
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Category category = unitOfWork.Category.FindByID(id);
                unitOfWork.Category.Delete(category);
                unitOfWork.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Index()
        {
            List<Category> categories = unitOfWork.Category.GetAll();
            return View("Index", categories);
        }
    }
}
