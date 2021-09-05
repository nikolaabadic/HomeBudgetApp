using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeBudgetApp.Data.UnitOfWork;
using HomeBudgetApp.Domain;
using HomeBudgetApp.WebApp.Filters;
using HomeBudgetApp.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBudgetApp.WebApp.Controllers
{
    public class PaymentCardController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public PaymentCardController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        // GET: PaymentCardController/Create
        [LoggedInUser]
        public ActionResult Create(int id)
        {
            PaymentCardCreateModel model = new PaymentCardCreateModel{
                AccountID = id
            };
            return View("Create", model);
        }

        // POST: PaymentCardController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoggedInUser]
        public ActionResult Create(PaymentCardCreateModel model)
        {
            PaymentCard card = new PaymentCard { PaymentCardNumber = model.PaymentCardNumber, PINCode = model.PINCode };
            
            try
            {
                Account account = unitOfWork.Account.FindByID(model.AccountID);
                account.PaymentCards.Add(card);
                unitOfWork.Account.Edit(account);
                unitOfWork.Commit();
                return RedirectToAction("ShowDetails", "Account", new { id = model.AccountID});
            }
            catch
            {
                return View("Create", model);
            }
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(int id, int PaymentCardID)
        {
            Account account = unitOfWork.Account.FindByID(id);
            PaymentCard card = null;
            foreach(PaymentCard c in account.PaymentCards)
            {
                if(c.PaymentCardID == PaymentCardID)
                {
                    card = c;
                }
            }
            if (card != null)
            {
                account.PaymentCards.Remove(card);
            }
            try
            {
                unitOfWork.Account.Edit(account);
                unitOfWork.Commit();                
            } catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);                
            }
            return RedirectToAction("ShowDetails", "Account", new { id = id });
        }
    }
}
