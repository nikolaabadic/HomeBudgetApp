using HomeBudgetApp.Domain;
using HomeBudgetApp.Data.Implementation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgetApp.Data.UnitOfWork.Implementation
{
    public class HomeBudgetUnitOfWork : IUnitOfWork
    {
        private readonly HomeBudgetContext context;

        public HomeBudgetUnitOfWork(HomeBudgetContext context)
        {
            this.context = context;
            User = new RepositoryUser(context);
            Account = new RepositoryAccount(context);
            Transaction = new RepositoryTransaction(context);
            Category = new RepositoryCategory(context);
            TransactionCategory = new RepositoryTransactionCategory(context);
            Template = new RepositoryTemplate(context);
            TransactionAccount = new RepositoryTransactionAccount(context);
        }

        public IRepositoryUser User { get; set; }
        public IRepositoryAccount Account { get; set; }
        public IRepositoryTransaction Transaction { get; set; }
        public IRepositoryCategory Category { get; set; }
        public IRepositoryTransactionCategory TransactionCategory { get; set; }
        public IRepositoryTransactionAccount TransactionAccount { get; set; }
        public IRepositoryTemplate Template { get; set; }

        public void Commit()
        {
            context.SaveChanges();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
