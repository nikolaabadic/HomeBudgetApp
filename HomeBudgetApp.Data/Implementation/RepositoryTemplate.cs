using HomeBudgetApp.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HomeBudgetApp.Data.Implementation
{
    public class RepositoryTemplate : IRepositoryTemplate
    {
        private readonly HomeBudgetContext context;
        public RepositoryTemplate(HomeBudgetContext context)
        {
            this.context = context;
        }
        public void Add(Template template)
        {
            context.Templates.Add(template);
        }

        public void Delete(Template template)
        {
            context.Templates.Remove(template);
        }

        public Template FindByID(int id, params int[] ids)
        {
            return context.Templates.Find(id);            
        }

        public List<Template> GetAll()
        {
            return context.Templates.ToList();
        }

        public List<Template> Search(Expression<Func<Template, bool>> pred)
        {
            return context.Templates.Where(pred).AsNoTracking().ToList();            
        }
        public void Edit(Template template)
        {
            context.Templates.Update(template);
        }
    }
}
