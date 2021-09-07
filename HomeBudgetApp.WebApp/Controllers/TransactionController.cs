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
            catch (Exception)
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
            catch (Exception)
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
                TransactionCreateModel model = new TransactionCreateModel { Categories = categories, AccountNumber = accountNumber, AccountID = (int)id };
                return View("Create", model);
            }
            return RedirectToAction("ShowDetails", "Account", new { id });
        }

        [LoggedInUser]
        public ActionResult CreateFromTemplate(TransactionFromTemplateModel m)
        {
            List<Category> list = unitOfWork.Category.GetAll();
            List<SelectListItem> categories = list.Select(c => new SelectListItem { Text = c.Name, Value = c.CategoryID.ToString() }).ToList();

            Account account = unitOfWork.Account.Search(a => a.Number.Equals(m.AccountNumber))[0];

            TransactionCreateModel createModel = new TransactionCreateModel
            {
                AccountID = account.AccountID,
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

                if(model.Transaction.DateTime == new DateTime())
                {
                    model.Transaction.DateTime = DateTime.UtcNow;
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
                    };
                    HttpContext.Session.Set("transaction", JsonSerializer.SerializeToUtf8Bytes(t));
                    TemplateCreateModel newModel = new TemplateCreateModel
                    {

                        RecipientAccountNumber = model.RecipientAccountNumber,
                        AccountNumber = model.AccountNumber

                    };
                    return RedirectToAction("Create", "Template",newModel);
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

                TransactionCreateModel model = new TransactionCreateModel
                {
                    AccountID = (int)ownerAccountID,
                    Transaction = transaction,
                    Categories = list,
                    CategoryList = categories
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


                if(model.Transaction.AccountNumber != transaction.AccountNumber)
                {
                    Account account = unitOfWork.Account.FindByNumber(model.Transaction.AccountNumber);
                    transaction.AccountNumber = model.Transaction.AccountNumber;
                    if (account == null)
                    {                        
                        transaction.AccountID = null;
                    }
                    else
                    {                        
                        transaction.AccountID = account.AccountID;
                    }
                }

                if(model.Transaction.RecipientAccountNumber != transaction.RecipientAccountNumber)
                {
                    Account recipient = unitOfWork.Account.FindByNumber(model.Transaction.RecipientAccountNumber);
                    transaction.RecipientAccountNumber = model.Transaction.RecipientAccountNumber;
                    if(recipient == null)
                    {
                        transaction.RecipientID = null;
                    }
                    else
                    {
                        transaction.RecipientID = recipient.AccountID;
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
                return RedirectToAction("ShowDetails", "Account", new { id });
            }
            catch(Exception)
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
                };
                return View("Search", model);
            } 
            AccountDetailsModel result = JsonSerializer.Deserialize<AccountDetailsModel>(modelByte);
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
                transactions = transactions.Where(t => t.Type == "income").ToList();
            }
            if(type == 2)
            {
                transactions = transactions.Where(t => t.Type == "expense").ToList();
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
