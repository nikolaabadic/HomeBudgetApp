using HomeBudgetApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HomeBudgetApp.Data.Implementation
{
    public class RepositoryAdmin : IRepositoryAdmin
    {
        private readonly AdminContext context;

        public RepositoryAdmin(AdminContext context)
        {
            this.context = context;
        }
        public void Add(Admin admin)
        {
            context.Admin.Add(admin);
        }

        public void Delete(Admin admin)
        {
            context.Admin.Remove(admin);
        }

        public Admin FindByID(int id, params int[] ids)
        {
            return context.Admin.Find(id);
        }

        public List<Admin> GetAll()
        {
            return context.Admin.ToList();
        }

        public Admin GetByUsernameAndPinCode(Admin admin)
        {
            return context.Admin.Single(a => a.Username == admin.Username && a.Password == admin.Password);
        }

        public Admin Search(Expression<Func<Admin, bool>> pred)
        {
            return context.Admin.Single(pred);
        }
    }
}
