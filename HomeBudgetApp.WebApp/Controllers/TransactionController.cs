﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using HomeBudgetApp.Domain;
using HomeBudgetApp.Data.UnitOfWork;
using HomeBudgetApp.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HomeBudgetApp.WebApp.Filters;

namespace HomeBudgetApp.WebApp.Controllers
{
    [LoggedInUser]
    public class TransactionController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public TransactionController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowDetails(int id, int accountID, int recipientID, int ownerAccountID)
        {
            HttpContext.Session.SetInt32("paymentid", id);
            HttpContext.Session.SetInt32("recipientid", recipientID);
            HttpContext.Session.SetInt32("paymentaccountid", accountID);
            HttpContext.Session.SetInt32("owneraccountid", ownerAccountID);
            return RedirectToAction("Details");
        }

        public ActionResult Details()
        {
            int? id = HttpContext.Session.GetInt32("paymentid");
            int? recipientID = HttpContext.Session.GetInt32("recipientid");
            int? accountID = HttpContext.Session.GetInt32("paymentaccountid");
            int? ownerAccountID = HttpContext.Session.GetInt32("owneraccountid");

            if (id != null && recipientID != null && accountID != null && ownerAccountID != null)
            {
                List<TransactionCategory> belongings = unitOfWork.TransactionCategory.Search(b => b.TransactionID == (int)id && b.OwnerID == (int)ownerAccountID);
                TransactionDetailsModel model = new TransactionDetailsModel();
                model.Transaction = unitOfWork.Transaction.FindByID((int)id);
                model.Transaction.Categories = belongings;
                model.Categories = unitOfWork.Category.GetAll();
                model.AccountNumber = unitOfWork.Account.Search(a => a.AccountID == accountID)[0].Number;
                model.RecipientNumber = unitOfWork.Account.Search(a => a.AccountID == recipientID)[0].Number;
                return View(model);
            }
            return RedirectToAction("Details", "Account");
        }

        [HttpPost]
        public ActionResult AddCategory(TransactionCategoryModel request)
        {
            try
            {
                Category category = unitOfWork.Category.FindByID(request.CategoryID);
                TransactionCategoryModel model = new TransactionCategoryModel
                {
                    Num = request.Num,
                    CategoryID = request.CategoryID,
                    Name = category.Name,
                    OwnerID = request.OwnerID
                };
                unitOfWork.Commit();
                return PartialView("TransactionCategoryPartialView", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error adding categories!");
                return View("Edit");
            }
        }

        [HttpPost]
        public ActionResult AddCategoryEdit(TransactionCategoryEditModel request)
        {
            try
            {
                Category category = unitOfWork.Category.FindByID(request.CategoryID);
                TransactionCategoryModel model = new TransactionCategoryModel
                {
                    Num = request.Num,
                    CategoryID = request.CategoryID,
                    Name = category.Name,
                    OwnerID = request.OwnerID
                };
                TransactionCategory b = new TransactionCategory
                {
                    CategoryID = category.CategoryID,
                    TransactionID = request.TransactionID,
                    OwnerID = request.OwnerID
                };
                unitOfWork.TransactionCategory.Add(b);
                unitOfWork.Commit();
                return PartialView("TransactionCategoryPartialView", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error adding categories!");
                return PartialView("ErrorPatrialView");
            }
        }

        [HttpDelete]
        public ActionResult DeleteCategory(int id, int accountID, int recipientID, int categoryID, int ownerID)
        {
            try
            {
                Transaction transaction = unitOfWork.Transaction.FindByID(id);
                TransactionCategory tc = unitOfWork.TransactionCategory.FindByID(categoryID, id, accountID, recipientID, ownerID);
                transaction.Categories.Remove(tc);
                unitOfWork.Commit();
                return RedirectToAction("Details", "Account");
            }
            catch (Exception e)
            {
                return RedirectToAction("Details", "Account");
            }
        }

        public ActionResult Create()
        {
            List<Category> list = unitOfWork.Category.GetAll();
            List<SelectListItem> categories = list.Select(c => new SelectListItem { Text = c.Name, Value = c.CategoryID.ToString() }).ToList();

            int? id = HttpContext.Session.GetInt32("accountid");
            if (id != null)
            {
                string accountNumber = unitOfWork.Account.Search(a => a.AccountID == (int)id)[0].Number;
                TransactionCreateModel model = new TransactionCreateModel { Categories = categories, AccountNumber = accountNumber, AccountID = (int)id };
                return View("Create", model);
            }
            return RedirectToAction("ShowDetails", "Account", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TransactionCreateModel model)
        {
            try
            {
                try
                {
                    Account recipient = unitOfWork.Account.Search(a => a.Number == model.RecipientAccountNumber)[0];
                    model.Transaction.RecipientID = recipient.AccountID;

                    Account account = unitOfWork.Account.FindByID(model.Transaction.AccountID);
                    if (account.Currency != recipient.Currency)
                    {
                        throw new FormatException();
                    }
                    byte[] amountByte = HttpContext.Session.Get("amount");
                    double amount = JsonSerializer.Deserialize<double>(amountByte);
                    if (model.Transaction.Amount > amount)
                    {
                        //throw new ApplicationException();
                    }
                }
                catch (FormatException)
                {
                    throw new Exception("The recipient account you entered is in another currency!");
                }
                catch (ApplicationException)
                {
                    throw new Exception("There is not enough money on the account for this payment!");
                }
                catch (Exception)
                {
                    throw new Exception("Invalid recipient account number!");
                }


                if (model.Transaction.Amount == 0)
                {
                    throw new Exception("Payment amount can't be 0!");
                }

                unitOfWork.Transaction.Add(model.Transaction);
                unitOfWork.Commit();
                return RedirectToAction("ShowDetails", "Account", new { id = model.Transaction.AccountID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty,
                    ex.Message.Length > 100 ? "Error adding categories!" : ex.Message);
                int? id = HttpContext.Session.GetInt32("accountid");
                return View("Create", new TransactionCreateModel
                {
                    AccountID = model.Transaction.AccountID,
                    AccountNumber = unitOfWork.Account.Search(a => a.AccountID == (int)id)[0].Number,
                    Categories = unitOfWork.Category.GetAll().Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.CategoryID.ToString()
                    }).ToList(),
                    Transaction = model.Transaction
                });
            }
        }

        public ActionResult ShowEdit(int id, int accountID, int recipientID, int ownerAccountID)
        {
            HttpContext.Session.SetInt32("paymentid", id);
            HttpContext.Session.SetInt32("recipientid", recipientID);
            HttpContext.Session.SetInt32("paymentaccountid", accountID);
            HttpContext.Session.SetInt32("owneraccountid", ownerAccountID);
            return RedirectToAction("Edit");
        }
        public ActionResult Edit()
        {
            int? id = HttpContext.Session.GetInt32("paymentid");
            int? recipientID = HttpContext.Session.GetInt32("recipientid");
            int? accountID = HttpContext.Session.GetInt32("paymentaccountid");
            int? ownerAccountID = HttpContext.Session.GetInt32("owneraccountid");

            if (id != null && recipientID != null && accountID != null && ownerAccountID != null)
            {
                Transaction transaction = unitOfWork.Transaction.FindByID((int)id);
                transaction.Categories = unitOfWork.TransactionCategory.Search(b => b.TransactionID == (int)id && b.OwnerID == (int)ownerAccountID);
                List<Category> categories = unitOfWork.Category.GetAll();
                List<SelectListItem> list = categories.Select(c => new SelectListItem { Text = c.Name, Value = c.CategoryID.ToString() }).ToList();

                string accountNum = unitOfWork.Account.FindByID((int)accountID).Number;
                string recipientNum = unitOfWork.Account.FindByID((int)recipientID).Number;
                TransactionCreateModel model = new TransactionCreateModel
                {
                    AccountID = (int)ownerAccountID,
                    Transaction = transaction,
                    Categories = list,
                    CategoryList = categories,
                    AccountNumber = accountNum,
                    RecipientAccountNumber = recipientNum
                };
                return View(model);
            }
            return RedirectToAction("ShowDetails", "Account", new { id = accountID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TransactionCreateModel model)
        {
            try
            {
                unitOfWork.Transaction.UpdateCategoryList(model.Transaction, model.Transaction.Categories);

                Transaction transaction = unitOfWork.Transaction.FindByID(model.Transaction.TransactionID);
                transaction.RecipientName = model.Transaction.RecipientName;
                transaction.RecipientAddress = model.Transaction.RecipientAddress;
                transaction.Purpose = model.Transaction.Purpose;
                unitOfWork.Transaction.Edit(transaction);
                unitOfWork.Commit();
                int id = model.AccountID;
                return RedirectToAction("ShowDetails", "Account", new { id });
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Error editing categories!");
                return RedirectToAction("Edit");
            }
        }

        [HttpGet]
        public ActionResult Search()
        {
            int id = (int)HttpContext.Session.GetInt32("accountid");

            byte[] modelByte = HttpContext.Session.Get("model");
            if (modelByte == null)
            {
                List<Transaction> paymentsFrom = unitOfWork.Transaction.Search(p => p.RecipientID == id);
                List<Transaction> paymentsTo = unitOfWork.Transaction.Search(p => p.AccountID == id);
                List<Transaction> finalList = paymentsFrom.Concat(paymentsTo).ToList().OrderBy(p => p.DateTime).Reverse().ToList();
                AccountDetailsModel model = new AccountDetailsModel
                {
                    OwnerAccountID = id,
                    Transactions = finalList,
                    Accounts = unitOfWork.Account.GetAll()
                };
                return View("Search", model);
            } 
            AccountDetailsModel result = JsonSerializer.Deserialize<AccountDetailsModel>(modelByte);
            result.Accounts = unitOfWork.Account.GetAll();
            return View("Search", result);
        }


        public ActionResult GetResults(string Param, string Type, string DateOrder)
        {
            int id = (int)HttpContext.Session.GetInt32("accountid");
            List<Transaction> transactions = unitOfWork.Transaction.Search(t =>
                (t.AccountID == id || t.RecipientID == id) && (
                t.Purpose.ToLower().Contains(Param.ToLower()) ||
                t.RecipientName.ToLower().Contains(Param.ToLower()) ||
                t.RecipientAddress.ToLower().Contains(Param.ToLower()) ||
                t.Recipient.Number.ToLower().Contains(Param.ToLower()) ||
                t.Account.Number.ToLower().Contains(Param.ToLower())));

            int type = int.Parse(Type);
            if (String.IsNullOrEmpty(Param))
            {
                transactions = unitOfWork.Transaction.Search(t => t.AccountID == id || t.RecipientID == id).OrderBy(t => t.DateTime).Reverse().ToList();
            } else
            {
                transactions = transactions.OrderBy(t => t.DateTime).Reverse().ToList();
            }

            if (type == 1)
            {
                transactions = transactions.Where(t => t.RecipientID == id).ToList();
            }
            if(type == 2)
            {
                transactions = transactions.Where(t => t.AccountID == id).ToList();
            }

            if(int.Parse(DateOrder) == 2)
            {
                transactions = transactions.OrderBy(t => t.DateTime).ToList();
            }
            AccountDetailsModel model = new AccountDetailsModel
            {
                OwnerAccountID = id,
                Transactions = transactions,
                Query = Param
            };
            HttpContext.Session.Set("model", JsonSerializer.SerializeToUtf8Bytes(model));

            return Json(new { redirectToUrl = Url.Action("Search") });
        }
    }
}