using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgetApp.Domain
{
    public class PaymentCard
    {
        public int PaymentCardID { get; set; }
        public string PaymentCardNumber { get; set; }
        public string PINCode { get; set; }
    }
}
