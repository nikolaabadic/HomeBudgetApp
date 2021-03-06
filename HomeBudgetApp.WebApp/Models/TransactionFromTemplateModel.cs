using HomeBudgetApp.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBudgetApp.WebApp.Models
{
    public class TransactionFromTemplateModel
    {
        public string AccountNumber { get; set; }
        public string RecipientAccountNumber { get; set; }
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
        public string Purpose { get; set; }
        public int Model { get; set; }
        public string ReferenceNumber { get; set; }
        public double Amount { get; set; }
        public string Type { get; set; }
        public List<SelectListItem> Categories { get; set; }

        public DateTime Now = DateTime.UtcNow;
    }
}
