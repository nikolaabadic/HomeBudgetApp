using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgetApp.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        public IRepositoryUser User { get; set; }
        public IRepositoryAccount Account { get; set; }
        public IRepositoryTransaction Transaction { get; set; }
        public IRepositoryCategory Category { get; set; }
        public IRepositoryTransactionCategory TransactionCategory { get; set; }
        public IRepositoryTransactionAccount TransactionAccount { get; set; }
        public IRepositoryTemplate Template { get; set; }
        void Commit();
    }
}
