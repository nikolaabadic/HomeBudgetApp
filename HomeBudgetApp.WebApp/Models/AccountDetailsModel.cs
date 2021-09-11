using HomeBudgetApp.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBudgetApp.WebApp.Models
{
    public class AccountDetailsModel
    {
        public int OwnerAccountID { get; set; }
        public Currency Currency { get; set; }
        public AccountType AccountType { get; set; }
        public string Number { get; set; }
        public double Amount { get; set; }
        public List<PaymentCard> PaymentCards { get; set; }
        public List<Transaction> Transactions { get; set; }
        public string Username { get; set; }
        public int UserID { get; set; }
        public string Query { get; set; }
        public List<double> IncomeCategory { get; set; }
        public List<double> ExpenseCategory { get; set; }
        public List<string> IncomeLabels { get; set; }
        public List<string> ExpenseLabels { get; set; }
        public List<SelectListItem> Categories { get; set; }
    }
}
