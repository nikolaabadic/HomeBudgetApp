using HomeBudgetApp.Domain;
using HomeBudgetApp.Data.Implementation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgetApp.Data.UnitOfWork.Implementation
{
    public class AdminUnitOfWork : IAdminUnitOfWork
    {
        private readonly AdminContext context;

        public AdminUnitOfWork(AdminContext context)
        {
            this.context = context;
            Admin = new RepositoryAdmin(context);
        }
        public IRepositoryAdmin Admin { get; set; }

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
