﻿using System;
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
            List<Template> templates = unitOfWork.Template.Search(t => t.UserID == id).OrderBy(t => t.Name).ToList();
            List<Category> categories = unitOfWork.Category.GetAll();
            return View(new TemplatesModel{
                Templates = templates,
                Categories = categories
            });
        }

        public ActionResult Create(TemplateCreateModel m)
        {
            int id = (int)HttpContext.Session.GetInt32("userid");
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
            
            return View(m);
        }

        [HttpPost]
        public ActionResult CreateTemplate(TemplateCreateModel m)
        {
            User user = unitOfWork.User.FindByID(m.UserID);
            Category category = unitOfWork.Category.FindByID(m.CategoryID);

            List<Template> templates = unitOfWork.Template.Search(t =>
                t.UserID == m.UserID && t.Name == m.Name);
            if(templates == null || templates.Count == 0)
            {
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
                    AccountNumber = m.Transaction.AccountNumber
                });
                unitOfWork.Commit();
                return RedirectToAction("Details", "Account");
            } else
            {
                ModelState.AddModelError(string.Empty,"Please enter a unique template name.");
                m.Categories = unitOfWork.Category.GetAll()
                .Select(c => new SelectListItem { Text = c.Name, Value = c.CategoryID.ToString() }).ToList();
                return View("Create", m);
            }            
        }

        public ActionResult Delete(int id)
        {
            Template template = unitOfWork.Template.FindByID(id);
            unitOfWork.Template.Delete(template);
            unitOfWork.Commit();

            return RedirectToAction("Index");
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
            return RedirectToAction("Index");
        }
    }
}
