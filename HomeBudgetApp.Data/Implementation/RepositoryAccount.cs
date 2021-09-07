using HomeBudgetApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HomeBudgetApp.Data.Implementation
{
    public class RepositoryAccount : IRepositoryAccount
    {
        private readonly HomeBudgetContext context;
        public RepositoryAccount(HomeBudgetContext context)
        {
            this.context = context;
        }
        public void Add(Account account)
        {
            context.Accounts.Add(account);
        }

        public void Delete(Account account)
        {
            context.Accounts.Remove(account);
        }

        public Account FindByID(int id, params int[] ids)
        {
            return context.Accounts.Find(id);
        }

        public List<Account> GetAll()
        {
            return context.Accounts.ToList();
        }

        public List<Account> Search(Expression<Func<Account, bool>> pred)
        {
            return context.Accounts.Where(pred).ToList();
        }

        public void Edit(Account account)
        {
            context.Accounts.Update(account);
        }

        public Account FindByNumber(string number)
        {
            try
            {
                Account account = context.Accounts.Single(a => a.Number == number);
                return account;
            } catch (Exception)
            {
                return null;
            }

        }
    }
}
