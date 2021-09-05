using HomeBudgetApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HomeBudgetApp.Data
{
    public interface IRepositoryTransactionCategory : IRepository<TransactionCategory>
    {
        public List<TransactionCategory> Search(Expression<Func<TransactionCategory, bool>> pred);
    }
}
