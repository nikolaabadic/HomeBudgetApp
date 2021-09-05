using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgetApp.Domain
{
    public class Transaction
    {
        public int TransactionID { get; set; }
        public DateTime DateTime { get; set; }
        public int AccountID { get; set; }
        public Account Account { get; set; }
        public int RecipientID { get; set; }
        public Account Recipient { get; set; }
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
        public string Purpose { get; set; }
        public int Model { get; set; }
        public string ReferenceNumber { get; set; }
        public double Amount { get; set; }
        public string Type { get; set; }
        public int? PaymentCardID { get; set; }
        public PaymentCard PaymentCard { get; set; }
        public List<TransactionCategory> Categories { get; set; }
    }
}
