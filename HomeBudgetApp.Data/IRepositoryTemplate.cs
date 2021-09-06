using HomeBudgetApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HomeBudgetApp.Data
{
    public interface IRepositoryTemplate : IRepository<Template>
    {
        public List<Template> Search(Expression<Func<Template, bool>> pred);
        public void Edit(Template template);
    }
}
