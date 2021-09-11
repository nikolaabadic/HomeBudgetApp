using System;
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
using Newtonsoft.Json;

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
                model.OwnerAccountID = (int)ownerAccountID;
                return View(model);
            }
            return RedirectToAction("Details", "Account");
        }

        [HttpPost]
        public ActionResult AddCategory(int CategoryID, int Num, int OwnerID)
        {
            try
            {
                Category category = unitOfWork.Category.FindByID(CategoryID);
                TransactionCategoryModel model = new TransactionCategoryModel
                {
                    Num = Num,
                    CategoryID = CategoryID,
                    Name = category.Name,
                    OwnerID = OwnerID
                };
                unitOfWork.Commit();
                return PartialView("TransactionCategoryPartialView", model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Error adding categories!");
                return View("Edit");
            }
        }

        [HttpPost]
        public ActionResult AddCategoryEdit(int CategoryID, int Num, int OwnerID, int TransactionID)
        {
            try
            {
                Category category = unitOfWork.Category.FindByID(CategoryID);
                TransactionCategoryModel model = new TransactionCategoryModel
                {
                    Num = Num,
                    CategoryID = CategoryID,
                    Name = category.Name,
                    OwnerID = OwnerID
                };
                TransactionCategory b = new TransactionCategory
                {
                    CategoryID = category.CategoryID,
                    TransactionID = TransactionID,
                    OwnerID = OwnerID
                };
                unitOfWork.TransactionCategory.Add(b);
                unitOfWork.Commit();
                return PartialView("TransactionCategoryPartialView", model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Error adding categories!");
                return PartialView("ErrorPatrialView");
            }
        }

        [HttpDelete]
        public ActionResult DeleteCategory(int id, int categoryID, int ownerID)
        {
            try
            {
                Transaction transaction = unitOfWork.Transaction.FindByID(id);
                TransactionCategory tc = unitOfWork.TransactionCategory.FindByID(categoryID, id, ownerID);
                transaction.Categories.Remove(tc);
                unitOfWork.Commit();
                return RedirectToAction("Details", "Account");
            }
            catch (Exception)
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
                TransactionCreateModel model = new TransactionCreateModel { 
                    Categories = categories, 
                    AccountNumber = accountNumber, 
                    AccountID = (int)id
                };
                return View("Create", model);
            }
            return RedirectToAction("ShowDetails", "Account", new { id });
        }

        [LoggedInUser]
        public ActionResult CreateFromTemplate(TransactionFromTemplateModel m)
        {
            List<Category> list = unitOfWork.Category.GetAll();
            List<SelectListItem> categories = list.Select(c => new SelectListItem { Text = c.Name, Value = c.CategoryID.ToString() }).ToList();

            Account account = unitOfWork.Account.FindByNumber(m.AccountNumber);
            int accountID = (account == null) ? 0 : account.AccountID;

            Account recipient = unitOfWork.Account.FindByNumber(m.RecipientAccountNumber);
            int recipientID = (recipient == null) ? 0 : recipient.AccountID;

            int id = m.Type == "expense" ? accountID : recipientID;
            TransactionCreateModel createModel = new TransactionCreateModel
            {
                AccountID = id,
                Categories = categories,
                AccountNumber = m.AccountNumber,
                RecipientAccountNumber = m.AccountNumber,
                Transaction = new Transaction
                {
                    RecipientName = m.RecipientName,
                    RecipientAddress = m.RecipientAddress,
                    Purpose = m.Purpose,
                    Model = m.Model,
                    ReferenceNumber = m.ReferenceNumber,
                    Amount = m.Amount
                }
            };


            HttpContext.Session.SetInt32("accountid", id);
            createModel.Type = m.Type;
            return View("Create", createModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TransactionCreateModel model)
        {
            try
            {
                try
                {
                    Account recipient = unitOfWork.Account.FindByNumber(model.RecipientAccountNumber);
                    if(recipient != null)
                    {
                        model.Transaction.RecipientID = recipient.AccountID;
                        model.Transaction.RecipientAccountNumber = recipient.Number;
                    }
                    else
                    {
                        model.Transaction.RecipientAccountNumber = model.RecipientAccountNumber;
                    }

                    Account account = unitOfWork.Account.FindByNumber(model.AccountNumber);
                    if(account != null)
                    {
                        model.Transaction.AccountID = account.AccountID;
                        model.Transaction.AccountNumber = account.Number;
                    }else
                    {
                        model.Transaction.AccountNumber = model.AccountNumber;
                    }

                    if (account!= null && recipient != null && account.Currency != recipient.Currency)
                    {
                        throw new FormatException();
                    }
                }
                catch (FormatException)
                {
                    throw new Exception("The recipient account you entered is in another currency!");
                }

                if (model.Transaction.Amount == 0)
                {
                    throw new Exception("Payment amount can't be 0!");
                }

                int transactionID = unitOfWork.Transaction.CreateAndReturnID(model.Transaction);
                
                if(model.Transaction.AccountID != null)
                {
                    unitOfWork.TransactionAccount.Add(new TransactionAccount
                    {
                        TransactionID = transactionID,
                        AccountID = (int)model.Transaction.AccountID,
                        Hidden = false
                    });
                }

                if (model.Transaction.RecipientID != null)
                {
                    unitOfWork.TransactionAccount.Add(new TransactionAccount
                    {
                        TransactionID = transactionID,
                        AccountID = (int)model.Transaction.RecipientID,
                        Hidden = false
                    });
                }

                unitOfWork.Commit();

                if (model.CreateTemplate)
                {
                    Transaction t = new Transaction
                    {
                        Purpose = model.Transaction.Purpose,
                        Model = model.Transaction.Model,
                        ReferenceNumber = model.Transaction.ReferenceNumber,
                        Amount = model.Transaction.Amount,
                        RecipientName = model.Transaction.RecipientName,
                        RecipientAddress = model.Transaction.RecipientAddress,
                        AccountNumber = model.Transaction.AccountNumber,
                        AccountID = model.Transaction.AccountID,
                        RecipientAccountNumber = model.Transaction.RecipientAccountNumber
                    };
                    HttpContext.Session.Set("transaction", System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(t));

                    return RedirectToAction("Create", "Template");
                }
                return RedirectToAction("ShowDetails", "Account", new { id = model.AccountID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty,
                    ex.Message.Length > 100 ? "Error adding categories!" : ex.Message);
                int? id = HttpContext.Session.GetInt32("accountid");
                return View("Create", new TransactionCreateModel
                {
                    AccountID = model.AccountID,
                    AccountNumber = model.AccountNumber,
                    Categories = unitOfWork.Category.GetAll().Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.CategoryID.ToString()
                    }).ToList(),
                    Transaction = model.Transaction
                });
            }
        }

        public ActionResult ShowEdit(int id, int ownerAccountID)
        {
            HttpContext.Session.SetInt32("paymentid", id);
            HttpContext.Session.SetInt32("owneraccountid", ownerAccountID);
            return RedirectToAction("Edit");
        }
        public ActionResult Edit()
        {
            int? id = HttpContext.Session.GetInt32("paymentid");
            int? ownerAccountID = HttpContext.Session.GetInt32("owneraccountid");

            if (id != null  && ownerAccountID != null)
            {
                Transaction transaction = unitOfWork.Transaction.FindByID((int)id);
                transaction.Categories = unitOfWork.TransactionCategory.Search(b => b.TransactionID == (int)id && b.OwnerID == (int)ownerAccountID);
                List<Category> categories = unitOfWork.Category.GetAll();
                List<SelectListItem> list = categories.Select(c => new SelectListItem { Text = c.Name, Value = c.CategoryID.ToString() }).ToList();

                TransactionCreateModel model = new TransactionCreateModel
                {
                    OwnerID = (int)ownerAccountID,
                    Transaction = transaction,
                    Categories = list,
                    CategoryList = categories
                };
                return View("Edit",model);
            }
            return RedirectToAction("ShowDetails", "Account", new { id = ownerAccountID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TransactionCreateModel model)
        {
            try
            {
                unitOfWork.Transaction.UpdateCategoryList(model.Transaction, model.Transaction.Categories);
                Transaction transaction = unitOfWork.Transaction.FindByID(model.Transaction.TransactionID);


                if(model.Transaction.AccountNumber != transaction.AccountNumber)
                {
                    Account account = unitOfWork.Account.FindByNumber(model.Transaction.AccountNumber);
                    transaction.AccountNumber = model.Transaction.AccountNumber;
                    if (account == null)
                    {
                        if (transaction.AccountID != null)
                        {
                            TransactionAccount obj = unitOfWork.TransactionAccount.FindByID((int)transaction.AccountID, transaction.TransactionID);
                            obj.Hidden = true;
                            unitOfWork.TransactionAccount.Edit(obj);
                        }
                        transaction.AccountID = null;
                    }
                    else
                    {                        
                        transaction.AccountID = account.AccountID;

                        TransactionAccount obj = unitOfWork.TransactionAccount.FindByID(account.AccountID, transaction.TransactionID);
                        if (obj == null)
                        {
                            unitOfWork.TransactionAccount.Add(new TransactionAccount { AccountID = account.AccountID, TransactionID = transaction.TransactionID, Hidden = false });
                        }
                        else
                        {
                            obj.Hidden = false;
                            unitOfWork.TransactionAccount.Edit(obj);
                        }

                    }
                }

                if(model.Transaction.RecipientAccountNumber != transaction.RecipientAccountNumber)
                {
                    Account recipient = unitOfWork.Account.FindByNumber(model.Transaction.RecipientAccountNumber);
                    transaction.RecipientAccountNumber = model.Transaction.RecipientAccountNumber;
                    if(recipient == null)
                    {
                        if(transaction.RecipientID != null)
                        {
                            TransactionAccount obj = unitOfWork.TransactionAccount.FindByID((int)transaction.RecipientID, transaction.TransactionID);
                            obj.Hidden = true;
                            unitOfWork.TransactionAccount.Edit(obj);
                        }
                        transaction.RecipientID = null;                        
                    }
                    else
                    {
                        transaction.RecipientID = recipient.AccountID;

                        TransactionAccount obj = unitOfWork.TransactionAccount.FindByID(recipient.AccountID, transaction.TransactionID);
                        if(obj == null)
                        {
                            unitOfWork.TransactionAccount.Add(new TransactionAccount { AccountID = recipient.AccountID, TransactionID = transaction.TransactionID, Hidden = false });
                        }
                        else
                        {
                            obj.Hidden = false;
                            unitOfWork.TransactionAccount.Edit(obj);
                        }
                    }
                }

                transaction.DateTime = model.Transaction.DateTime;
                transaction.RecipientName = model.Transaction.RecipientName;
                transaction.RecipientAddress = model.Transaction.RecipientAddress;
                transaction.Purpose = model.Transaction.Purpose;
                transaction.Model = model.Transaction.Model;
                transaction.ReferenceNumber = model.Transaction.ReferenceNumber;
                transaction.Amount = model.Transaction.Amount;

                unitOfWork.Transaction.Edit(transaction);
                unitOfWork.Commit();
                int id = model.AccountID;
                return RedirectToAction("Details", "Transaction");
            }
            catch(Exception)
            {
                ModelState.AddModelError(string.Empty, "Error editing categories!");
                return View("Edit", model);
            }
        }

        [HttpGet]
        public ActionResult Search(AccountDetailsModel model)
        {
            int id = (int)HttpContext.Session.GetInt32("accountid");
            model.OwnerAccountID = id;
            List<SelectListItem> list =unitOfWork.Category.GetAll().
                Select(c => new SelectListItem { Text = c.Name, Value = c.CategoryID.ToString() }).ToList();
            list.Insert(0, new SelectListItem { Text = "All", Value = "0" });

            if (TempData["model"] is string s)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<AccountDetailsModel>(s);   
                result.OwnerAccountID = id;
                result.Categories = list;
                return View("Search", result);
            }
            else 
            { 
                List<TransactionAccount> visibleTransactions = unitOfWork.TransactionAccount.Search(t => t.AccountID == id && !t.Hidden);
                List<Transaction> paymentsFrom = unitOfWork.Transaction.Search(p => p.RecipientID == id);
                paymentsFrom = paymentsFrom.Where(t => visibleTransactions.Any(p => p.TransactionID == t.TransactionID)).ToList();

                List<Transaction> paymentsTo = unitOfWork.Transaction.Search(p => p.AccountID == id);
                paymentsTo = paymentsTo.Where(t => visibleTransactions.Any(p => p.TransactionID == t.TransactionID)).ToList();
                List<Transaction> finalList = paymentsFrom.Concat(paymentsTo).ToList().OrderBy(p => p.DateTime).Reverse().ToList();
                
                model.Transactions = finalList;
                model.Categories = list;
                return View("Search", model);
            } 
        }


        public ActionResult GetResults(string Param, int Type, int DateOrder, int CategoryID)
        {
            int id = (int)HttpContext.Session.GetInt32("accountid");
            List<Transaction> transactions = unitOfWork.Transaction.Search(t =>
                (t.AccountID == id || t.RecipientID == id) && (
                t.Purpose.ToLower().Contains(Param.ToLower()) ||
                t.RecipientName.ToLower().Contains(Param.ToLower()) ||
                t.RecipientAddress.ToLower().Contains(Param.ToLower())));

            if (String.IsNullOrEmpty(Param))
            {
                transactions = unitOfWork.Transaction.Search(t => t.AccountID == id || t.RecipientID == id).OrderBy(t => t.DateTime).Reverse().ToList();
            } else
            {
                transactions = transactions.OrderBy(t => t.DateTime).Reverse().ToList();
            }

            if (Type == 1)
            {
                transactions = transactions.Where(t => t.RecipientID == id).ToList();
            }
            if(Type == 2)
            {
                transactions = transactions.Where(t => t.AccountID == id).ToList();
            } 
            if(DateOrder == 2)
            {
                transactions = transactions.OrderBy(t => t.DateTime).ToList();
            }

            List<TransactionAccount> visibleTransactions = unitOfWork.TransactionAccount.Search(t => t.AccountID == id && !t.Hidden);
            transactions = transactions.Where(t => visibleTransactions.Any(p => p.TransactionID == t.TransactionID)).ToList();

            List<TransactionCategory> categories = unitOfWork.TransactionCategory.Search(c => c.OwnerID == id && c.CategoryID == CategoryID);
            if(CategoryID != 0)
            {
                transactions = transactions.Where(t => categories.Any(c => c.TransactionID == t.TransactionID)).ToList();
            }

            AccountDetailsModel model = new AccountDetailsModel
            {
                OwnerAccountID = id,
                Transactions = transactions,
                Query = Param
            };

            TempData["model"] = Newtonsoft.Json.JsonConvert.SerializeObject(model, 
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            return Json(new { redirectToUrl = Url.Action("Search", "Transaction") });
        }

        [HttpPost]
        public ActionResult Delete(int transactionID, int accountID)
        {
            TransactionAccount obj = unitOfWork.TransactionAccount.FindByID(accountID, transactionID);
            obj.Hidden = true;
            unitOfWork.TransactionAccount.Edit(obj);
            unitOfWork.Commit();
            return Json(new { redirectToUrl = Url.Action("Details", "Account") });
        }
    }
}
