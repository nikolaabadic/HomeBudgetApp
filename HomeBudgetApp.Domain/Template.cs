using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgetApp.Domain
{
    public class Template
    {
        public int TemplateID { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
        public string RecipientAccountNumber { get; set; }
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
        public string Purpose { get; set; }
        public int Model { get; set; }
        public string ReferenceNumber { get; set; }
        public double Amount { get; set; }
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public Category Category { get; set; }
        public string Type { get; set; }
    }
}
