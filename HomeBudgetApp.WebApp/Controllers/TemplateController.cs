using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using HomeBudgetApp.Data.UnitOfWork;
using HomeBudgetApp.Domain;
using HomeBudgetApp.WebApp.Filters;
using HomeBudgetApp.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HomeBudgetApp.WebApp.Controllers
{
    [LoggedInUser]
    public class TemplateController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public TemplateController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public ActionResult Index()
        {
            int id = (int)HttpContext.Session.GetInt32("userid");
            List<Category> categories = unitOfWork.Category.GetAll();
            TemplatesModel model = new TemplatesModel
            {
                Categories = categories
            };

            byte[] templatesByte = HttpContext.Session.Get("templates");
            if (templatesByte == null)
            {
                model.Templates = unitOfWork.Template.Search(t => t.UserID == id).OrderBy(t => t.Name).ToList();
            } else
            {
                model.Templates = System.Text.Json.JsonSerializer.Deserialize<List<Template>>(templatesByte);
            }            

            return View("Index",model);
        }

        public ActionResult Create(TemplateCreateModel m)
        {
            int id = (int)HttpContext.Session.GetInt32("userid");
            int accountID = (int)HttpContext.Session.GetInt32("accountid");
            m.UserID = id;
            m.Categories = unitOfWork.Category.GetAll()
                .Select(c => new SelectListItem { Text = c.Name, Value = c.CategoryID.ToString() }).ToList();

            if (m.TransactionID != 0) {
                m.Transaction = unitOfWork.Transaction.FindByID(m.TransactionID);
            }
            else
            {
                byte[] transactionByte = HttpContext.Session.Get("transaction");
                m.Transaction = JsonSerializer.Deserialize<Transaction>(transactionByte);
            }

            m.Type = m.Transaction.AccountID == accountID ? "expense" : "income";
            
            return View(m);
        }

        [HttpPost]
        public ActionResult CreateTemplate(TemplateCreateModel m)
        {
            User user = unitOfWork.User.FindByID(m.UserID);
            int accountID = (int)HttpContext.Session.GetInt32("accountid");
            Category category = unitOfWork.Category.FindByID(m.CategoryID);

            List<Template> templates = unitOfWork.Template.Search(t =>
                t.UserID == m.UserID && t.Name == m.Name);
            if(templates == null || templates.Count == 0)
            {
                string type = m.Transaction.AccountID != null && m.Transaction.AccountID== accountID ? "expense" : "income";
                unitOfWork.Template.Add(new Template
                {
                    User = user,
                    RecipientAccountNumber = m.Transaction.RecipientAccountNumber,
                    RecipientName = m.Transaction.RecipientName,
                    RecipientAddress = m.Transaction.RecipientAddress,
                    Purpose = m.Transaction.Purpose,
                    Model = m.Transaction.Model,
                    ReferenceNumber = m.Transaction.ReferenceNumber,
                    Category = category,
                    Amount = m.Transaction.Amount,
                    Name = m.Name,
                    AccountNumber = m.Transaction.AccountNumber,
                    Type = type
                });
                unitOfWork.Commit();
                return RedirectToAction("Index");
            } else
            {
                ModelState.AddModelError(string.Empty,"Please enter a unique template name.");
                m.Categories = unitOfWork.Category.GetAll()
                .Select(c => new SelectListItem { Text = c.Name, Value = c.CategoryID.ToString() }).ToList();
                m.Type = m.Transaction.AccountID != null && m.Transaction.AccountID == accountID ? "expense" : "income";
                return View("Create", m);
            }            
        }

        public ActionResult Delete(int id)
        {
            try
            {
                Template template = unitOfWork.Template.FindByID(id);
                unitOfWork.Template.Delete(template);
                unitOfWork.Commit();

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View("Error", new ErrorViewModel
                 {
                        StatusCode = 500,
                        Message = "Error deleting template. Please try again later."                   
                });
            }
        }

        public ActionResult Edit(int id)
        {
            int userID = (int)HttpContext.Session.GetInt32("userid");
            Template template = unitOfWork.Template.FindByID(id);
            List<SelectListItem> categories = unitOfWork.Category.GetAll()
                .Select(c => new SelectListItem { Text = c.Name, Value = c.CategoryID.ToString() }).ToList();
            

            
            return View(new TemplateCreateModel
            {
                UserID = userID,
                Template = template,
                Categories = categories
            }); ;
        }

        [HttpPost]
        public ActionResult Edit(TemplateCreateModel m)
        {
            List<Template> templates = unitOfWork.Template.Search(t =>
                t.UserID == m.UserID && t.Name == m.Template.Name);

            if (templates != null && templates.Count == 1)
            {
                if(templates[0].TemplateID != m.Template.TemplateID)
                {
                    ModelState.AddModelError(string.Empty, "Please enter a unique template name.");
                    m.Categories = unitOfWork.Category.GetAll()
                    .Select(c => new SelectListItem { Text = c.Name, Value = c.CategoryID.ToString() }).ToList();

                    return View(m);
                }
            }

            m.Template.UserID = m.UserID;
            unitOfWork.Template.Edit(m.Template);
            unitOfWork.Commit();
            return RedirectToAction("Details", new { templateID = m.Template.TemplateID });
        }

       [HttpGet]
       public ActionResult Details(int templateID)
       {
            try
            {
                Template template = unitOfWork.Template.FindByID(templateID);
                template.Category = unitOfWork.Category.FindByID(template.CategoryID);
                TemplateCreateModel model = new TemplateCreateModel
                {
                    Template = template
                };
                return View(model);
            }
            catch (Exception)
            {
                return View("Error", new ErrorViewModel
                {
                    StatusCode = 404,
                    Message = "Error loading template. Please try again later."
                });
            }
       }

        [HttpPost]
        public ActionResult Search(string Param)
        {
            int id = (int)HttpContext.Session.GetInt32("userid");
            List<Template> templates = null;
            if (string.IsNullOrEmpty(Param))
            {
                templates = unitOfWork.Template.Search(t => t.UserID == id);
            } else
            {
                templates = unitOfWork.Template.Search(t => t.UserID == id && t.Name.ToLower().Contains(Param)).OrderBy(t => t.Name).ToList();
            }

            HttpContext.Session.Set("templates", System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(templates));

            return Json(new { redirectToUrl = Url.Action("Index") });
        }
    }
}
