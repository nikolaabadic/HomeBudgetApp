using HomeBudgetApp.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBudgetApp.WebApp.Models
{
    public class TemplateCreateModel
    {
        public int TransactionID { get; set; }
        public Transaction Transaction { get; set; }
        public int CategoryID { get; set; }
        public Category Category { get; set; }
        public string RecipientAccountNumber { get; set; }
        public string Name { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public int UserID { get; set; }
        public DateTime Now = DateTime.UtcNow;
        public string AccountNumber { get; set; }
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
        public string Purpose { get; set; }
        public int Model { get; set; }
        public string ReferenceNumber { get; set; }
        public double Amount { get; set; }
    }
}
