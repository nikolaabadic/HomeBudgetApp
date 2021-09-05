using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgetApp.Data.UnitOfWork
{
    public interface IAdminUnitOfWork : IDisposable
    {
        public IRepositoryAdmin Admin { get; set; }
        void Commit();
    }
}
