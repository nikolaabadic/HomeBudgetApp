using HomeBudgetApp.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBudgetApp.WebApp.Models
{
    public class TransactionCreateModel
    {
        public int AccountID { get; set; }
        public Transaction Transaction { get; set; }
        public string AccountNumber { get; set; }
        public string RecipientAccountNumber { get; set; }
        public List<Category> CategoryList { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public double TotalAmount { get; set; }
        public DateTime Now = DateTime.UtcNow;
    }
}
