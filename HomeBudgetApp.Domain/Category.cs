using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgetApp.Domain
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<TransactionCategory> Transactions { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ChartCategory
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
    }
}
