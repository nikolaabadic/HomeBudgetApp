using HomeBudgetApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HomeBudgetApp.Data
{
    public interface IRepositoryTransaction : IRepository<Transaction>
    {
        public List<Transaction> Search(Expression<Func<Transaction, bool>> pred);
        public void UpdateCategoryList(Transaction transaction, List<TransactionCategory> categories);
        public void Edit(Transaction transaction);
    }
}
