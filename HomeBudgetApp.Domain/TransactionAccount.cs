using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgetApp.Domain
{
    public class TransactionAccount
    {
        public int AccountID { get; set; }
        public Account Account { get; set; }
        public int TransactionID { get; set; }
        public Transaction Transaction { get; set; }
        public bool Hidden { get; set; }
    }
}
