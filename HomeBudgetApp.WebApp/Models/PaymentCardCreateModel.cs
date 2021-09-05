using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBudgetApp.WebApp.Models
{
    public class PaymentCardCreateModel
    {
        public int AccountID { get; set; }
        public string PaymentCardNumber { get; set; }
        public string PINCode { get; set; }
    }
}
