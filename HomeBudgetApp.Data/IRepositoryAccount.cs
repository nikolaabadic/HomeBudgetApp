using HomeBudgetApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HomeBudgetApp.Data
{
    public interface IRepositoryAccount : IRepository<Account>
    {
        public List<Account> Search(Expression<Func<Account, bool>> pred);
        public void Edit(Account account);
        public Account FindByNumber(string number);
    }
}
