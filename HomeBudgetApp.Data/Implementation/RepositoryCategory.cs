using HomeBudgetApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeBudgetApp.Data.Implementation
{
    public class RepositoryCategory : IRepositoryCategory
    {
        private readonly HomeBudgetContext context;
        public RepositoryCategory(HomeBudgetContext context)
        {
            this.context = context;
        }
        public void Add(Category category)
        {
            context.Categories.Add(category);
        }

        public void Delete(Category category)
        {
            context.Categories.Remove(category);
        }

        public Category FindByID(int id, params int[] ids)
        {
            return context.Categories.Find(id);
        }

        public List<Category> GetAll()
        {
            return context.Categories.ToList();
        }

        public Category FindByName(string name)
        {
            try
            {
                Category category = context.Categories.Single(c => c.Name == name);
                return category;
            } catch (Exception)
            {
                return null;
            }
        }
    }
}
