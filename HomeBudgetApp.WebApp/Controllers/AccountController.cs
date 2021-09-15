using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using HomeBudgetApp.Domain;
using HomeBudgetApp.Data.UnitOfWork;
using HomeBudgetApp.WebApp.Filters;
using HomeBudgetApp.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBudgetApp.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public AccountController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        // GET: AccountController
        [LoggedInUser]
        [AdminNotLoggedIn]
        public ActionResult Index()
        {
            return View();
        }
        [LoggedInUser]
        [AdminNotLoggedIn]
        public ActionResult ShowDetails(int id)
        {
            HttpContext.Session.SetInt32("accountid", id);
            return RedirectToAction("Details");
        }


        // GET: AccountController/Details/5
        [LoggedInUser]
        [AdminNotLoggedIn]
        public ActionResult Details()
        {
            try
            {
                int? id = HttpContext.Session.GetInt32("accountid");
                int? userID = HttpContext.Session.GetInt32("userid");
                if (id != null)
                {
                    Account account = unitOfWork.Account.FindByID((int)id);
                    AccountDetailsModel model = new AccountDetailsModel();

                    model.OwnerAccountID = (int)id;
                    model.AccountType = account.AccountType;
                    model.Currency = account.Currency;
                    model.Number = account.Number;
                    model.UserID = (int)userID;

                    double amount = 0;

                    List<TransactionAccount> visibleTransactions = unitOfWork.TransactionAccount.Search(t => t.AccountID == account.AccountID && !t.Hidden);
                    List<Transaction> paymentsFrom = unitOfWork.Transaction.Search(p => p.RecipientID == account.AccountID);
                    paymentsFrom = paymentsFrom.Where(t => visibleTransactions.Any(p => p.TransactionID == t.TransactionID)).ToList();

                    List<Transaction> paymentsTo = unitOfWork.Transaction.Search(p => p.AccountID == account.AccountID);
                    paymentsTo = paymentsTo.Where(t => visibleTransactions.Any(p => p.TransactionID == t.TransactionID)).ToList();

                    List<ChartCategory> chartCategoriesIncome = new List<ChartCategory>();
                    List<ChartCategory> chartCategoriesExpense = new List<ChartCategory>();

                    List<Category> categories = unitOfWork.Category.GetAll();

                    foreach (var c in categories)
                    {
                        foreach (var p in paymentsFrom)
                        {
                            try
                            {
                                List<TransactionCategory> belongings = unitOfWork.TransactionCategory.Search(b =>
                                    b.OwnerID == (int)id && p.TransactionID == b.TransactionID && b.CategoryID == c.CategoryID);
                                foreach (var b in belongings)
                                {
                                    chartCategoriesIncome.Add(new ChartCategory
                                    {
                                        CategoryID = b.CategoryID,
                                        Amount = p.Amount,
                                        Name = c.Name
                                    });
                                }
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                        }
                    }
                    foreach (var c in categories)
                    {
                        foreach (var p in paymentsTo)
                        {
                            try
                            {
                                List<TransactionCategory> belongings = unitOfWork.TransactionCategory.Search(
                                    b => b.OwnerID == (int)id && p.TransactionID == b.TransactionID &&
                                    b.CategoryID == c.CategoryID);
                                foreach (var b in belongings)
                                {
                                    chartCategoriesExpense.Add(new ChartCategory
                                    {
                                        CategoryID = b.CategoryID,
                                        Amount = p.Amount,
                                        Name = c.Name
                                    });
                                }
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                        }
                    }

                    List<string> categoryNamesIncome = chartCategoriesIncome.Select(cc => cc.Name).Distinct().ToList();
                    List<ChartCategory> incomeFinal = new List<ChartCategory>();
                    foreach (var c in categoryNamesIncome)
                    {
                        ChartCategory cc = new ChartCategory
                        {
                           Name = c,
                           Amount = 0
                        };

                        foreach (var p in chartCategoriesIncome)
                        {
                            if (p.Name == c)
                            {
                                cc.Amount += p.Amount;
                            }
                        }
                        incomeFinal.Add(cc);
                    }


                    List<string> categoryNamesExpense = chartCategoriesExpense.Select(cc => cc.Name).Distinct().ToList();
                    List<ChartCategory> expenseFinal = new List<ChartCategory>();
                    foreach (var c in categoryNamesExpense)
                    {
                        ChartCategory cc = new ChartCategory
                        {
                           Name = c,
                           Amount = 0
                        };

                        foreach (var p in chartCategoriesExpense)
                        {
                            if (p.Name == c)
                            {
                                cc.Amount += p.Amount;
                            }
                        }
                        expenseFinal.Add(cc);
                    }
                    model.IncomeLabels = categoryNamesIncome;
                    model.ExpenseLabels = categoryNamesExpense;
                    model.IncomeCategory = incomeFinal.Select(c => c.Amount).ToList();
                    model.ExpenseCategory = expenseFinal.Select(c => c.Amount).ToList();

                    foreach (var p in paymentsFrom)
                    {
                        amount += p.Amount;
                    }
                    foreach (var p in paymentsTo)
                    {
                        amount -= p.Amount;
                    }

                    model.Amount = amount;
                    model.Transactions = paymentsFrom.Concat(paymentsTo).ToList().OrderBy(p => p.DateTime).Reverse().ToList();
                    model.PaymentCards = account.PaymentCards;

                    HttpContext.Session.Set("amount", JsonSerializer.SerializeToUtf8Bytes(amount));
                    return View(model);
                }
                return RedirectToAction("Details", "User");
            }
            catch (Exception)
            {
                return View("Error", new ErrorViewModel
                {
                    StatusCode = 404,
                    Message = "Error loading account. Please try again later."
                });
            }

        }
        [LoggedInUser]
        // GET: AccountController/Create
        public ActionResult Create(int userID)
        {
            AccountDetailsModel model = new AccountDetailsModel
            {
                UserID = userID
            };
            return View("Create", model);
        }

        // POST: AccountController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoggedInUser]
        public ActionResult Create(AccountDetailsModel model)
        {

            if (model.Number == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid fields!");
                return View(model);
            }
            Account accountDB = unitOfWork.Account.FindByNumber(model.Number);
            if(accountDB != null)
            {
                ModelState.AddModelError(string.Empty, "Account number must be unique!");
                return View(model);
            }

            int userID = model.UserID;
            Account account = new Account
            {
                Currency = model.Currency,
                AccountType = model.AccountType,
                Number = model.Number,
                Amount = model.Amount,
                UserID = userID
            };
            unitOfWork.Account.Add(account);
            unitOfWork.Commit();
            return RedirectToAction("Details", "User");
        }

        [HttpPost]
        [LoggedInUser]
        public ActionResult Delete(int AccountID, int UserID)
        {
            try
            {
                Account account = unitOfWork.Account.FindByID(AccountID);
                account.Hidden = true;
                unitOfWork.Account.Edit(account);
                unitOfWork.Commit();
            }
            catch (Exception)
            {
                return Json(new
                {
                    redirectToUrl = Url.Action("ReturnError", "Account", new ErrorViewModel
                    {
                        StatusCode = 500,
                        Message = "Error deleting account. Please try again later."
                    })
                });
            }
            return Json(new { redirectToUrl = Url.Action("ShowDetails", "User", new { id = UserID }) });
        }

        [LoggedInUser]
        public ActionResult ReturnError(ErrorViewModel model)
        {
            return View("Error", model);
        }

        [LoggedInUser]
        public ActionResult Edit(int id, int userID)
        {
            Account account = unitOfWork.Account.FindByID(id);
            AccountDetailsModel model = new AccountDetailsModel
            {
                Currency = account.Currency,
                AccountType = account.AccountType,
                Number = account.Number,
                OwnerAccountID = id,
                UserID = userID
            };
            return View("Edit", model);
        }
        [HttpPost]
        [LoggedInUser]
        public ActionResult Edit(AccountDetailsModel model)
        {
            try {
                Account account = unitOfWork.Account.FindByID(model.OwnerAccountID);

                account.Currency = model.Currency;
                account.AccountType = model.AccountType;

                Account accountDB = unitOfWork.Account.FindByNumber(model.Number);
                if (accountDB == null || (accountDB != null && accountDB.AccountID == model.OwnerAccountID))
                {
                    account.Number = model.Number;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Account number must be unique!");
                    return View(model);
                }

                unitOfWork.Account.Edit(account);
                unitOfWork.Commit();
                return RedirectToAction("Details", "Account");

            } catch(Exception)
            {
                return View("Edit", model);
            }            
        }

        [HttpPost]
        public ActionResult Search(string Param)
        {
            int id = (int)HttpContext.Session.GetInt32("userid");
            List<Account> accounts = null;
            if (String.IsNullOrEmpty(Param))
            {
                accounts = unitOfWork.Account.Search(a => a.UserID == id && !a.Hidden);
            } else
            {
                accounts = unitOfWork.Account.Search(a => a.UserID == id && !a.Hidden && a.Number.ToLower().Contains(Param)).ToList();
            }
            

            HttpContext.Session.Set("accounts", System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(accounts));

            return Json(new { redirectToUrl = Url.Action("Details", "User") });
        }
    }
}
