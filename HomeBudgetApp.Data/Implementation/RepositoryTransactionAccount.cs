using HomeBudgetApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HomeBudgetApp.Data.Implementation
{
    public class RepositoryTransactionAccount : IRepositoryTransactionAccount
    {
        private readonly HomeBudgetContext context;
        public RepositoryTransactionAccount(HomeBudgetContext context)
        {
            this.context = context;
        }
        public void Add(TransactionAccount param)
        {
            context.TransactionAccounts.Add(param);
        }

        public void Delete(TransactionAccount param)
        {
            context.TransactionAccounts.Remove(param);
        }

        public TransactionAccount FindByID(int id, params int[] ids)
        {
            return context.TransactionAccounts.Find(id, ids[0]);
        }

        public List<TransactionAccount> GetAll()
        {
            return context.TransactionAccounts.ToList();
        }

        public List<TransactionAccount> Search(Expression<Func<TransactionAccount, bool>> pred)
        {
            return context.TransactionAccounts.Where(pred).ToList();
        }

        public void Edit(TransactionAccount param)
        {
            context.TransactionAccounts.Update(param);
        }
    }
}
