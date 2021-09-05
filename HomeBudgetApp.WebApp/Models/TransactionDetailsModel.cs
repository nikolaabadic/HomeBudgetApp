using HomeBudgetApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBudgetApp.WebApp.Models
{
    public class TransactionDetailsModel
    {
        public int OwnerAccountID { get; set; }
        public Transaction Transaction { get; set; }
        public List<Category> Categories { get; set; }
        public double Amount { get; set; }
        public string AccountNumber { get; set; }
        public string RecipientNumber { get; set; }
    }
}
