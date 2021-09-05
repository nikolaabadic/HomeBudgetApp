using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgetApp.Domain
{
    public class TransactionCategory
    {
        public int CategoryID { get; set; }
        public Category Category { get; set; }
        public int TransactionID { get; set; }
        public Transaction Transaction { get; set; }
        public int OwnerID { get; set; }
    }
}
