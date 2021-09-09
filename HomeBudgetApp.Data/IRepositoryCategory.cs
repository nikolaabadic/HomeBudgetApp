using HomeBudgetApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgetApp.Data
{
    public interface IRepositoryCategory : IRepository<Category>
    {
        public Category FindByName(string name);
    }
}
