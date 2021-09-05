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
        public string Name { get; set; }
        public int TransactionID { get; set; }
        public int AccountID { get; set; }
        public int RecipientID { get; set; }
    }
}
