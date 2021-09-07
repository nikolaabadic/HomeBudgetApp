using HomeBudgetApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HomeBudgetApp.Data
{
    public interface IRepositoryTransactionAccount : IRepository<TransactionAccount>
    {
        public List<TransactionAccount> Search(Expression<Func<TransactionAccount, bool>> pred);

        public void Edit(TransactionAccount param);
    }
}
