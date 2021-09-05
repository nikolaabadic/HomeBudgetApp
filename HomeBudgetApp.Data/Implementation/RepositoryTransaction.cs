using HomeBudgetApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HomeBudgetApp.Data.Implementation
{
    public class RepositoryTransaction : IRepositoryTransaction
    {
        private readonly HomeBudgetContext context;
        public RepositoryTransaction(HomeBudgetContext context)
        {
            this.context = context;
        }

        public void Add(Transaction transaction)
        {
            context.Transactions.Add(transaction);
        }

        public void Delete(Transaction transaction)
        {
            context.Transactions.Remove(transaction);
        }

        public Transaction FindByID(int id, params int[] ids)
        {
            return context.Transactions.Find(id);
        }

        public List<Transaction> GetAll()
        {
            return context.Transactions.ToList();
        }

        public List<Transaction> Search(Expression<Func<Transaction, bool>> pred)
        {
            List<Transaction> list = new List<Transaction>();
            try
            {
                list = context.Transactions.Where(pred).ToList();
            } catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return list;
        }
        public void UpdateCategoryList(Transaction transaction, List<TransactionCategory> categories)
        {
            Transaction t = FindByID(transaction.TransactionID);
            t.Categories = categories;
        }
        public void Edit(Transaction transaction)
        {
            context.Transactions.Update(transaction);
        }
    }
}
