using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HomeBudgetApp.Domain
{

        public enum Currency
        {
            RSD, EUR, USD, GBP
        }
        public enum AccountType
        {
            CURRENT, FOREIGN_EXCHANGE
        }

        public class Account
        {
            public int AccountID { get; set; }
            [Required(ErrorMessage = "You must enter all fields!")]
            public Currency Currency { get; set; }
            [Required(ErrorMessage = "You must enter all fields!")]
            public AccountType AccountType { get; set; }
            [Required(ErrorMessage = "You must enter all fields!")]
            public string Number { get; set; }
            public double Amount { get; set; }
            public double TotalIncome { get; set; }
            public double TotalExpense { get; set; }
            public int UserID { get; set; }
            public User User { get; set; }
            public List<PaymentCard> PaymentCards { get; set; }
            public List<Transaction> TransactionsFrom { get; set; }
            public List<Transaction> TransactionsTo { get; set; }
        }
}
