using HomeBudgetApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HomeBudgetApp.Data.Implementation
{
    public class RepositoryTransactionCategory : IRepositoryTransactionCategory
    {
        private readonly HomeBudgetContext context;
        public RepositoryTransactionCategory(HomeBudgetContext context)
        {
            this.context = context;
        }
        public void Add(TransactionCategory belonging)
        {
            context.TransactionCategories.Add(belonging);
        }

        public void Delete(TransactionCategory belonging)
        {
            context.TransactionCategories.Remove(belonging);
        }

        public TransactionCategory FindByID(int id, params int[] ids)
        {
            return context.TransactionCategories.Find(id, ids[0], ids[1]);
        }

        public List<TransactionCategory> GetAll()
        {
            return context.TransactionCategories.ToList();
        }

        public List<TransactionCategory> Search(Expression<Func<TransactionCategory, bool>> pred)
        {
            return context.TransactionCategories.Where(pred).ToList();
        }
    }
}
