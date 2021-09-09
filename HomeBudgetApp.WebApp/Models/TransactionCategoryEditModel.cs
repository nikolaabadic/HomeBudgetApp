using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBudgetApp.WebApp.Models
{
    public class TransactionCategoryEditModel
    {
        public int Num { get; set; }
        public int CategoryID { get; set; }
        public int OwnerID { get; set; }
        public int TransactionID { get; set; }
    }
}
