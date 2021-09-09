using HomeBudgetApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HomeBudgetApp.Data.Implementation
{
    public class RepositoryUser : IRepositoryUser
    {
        private readonly HomeBudgetContext context;
        public RepositoryUser(HomeBudgetContext context)
        {
            this.context = context;
        }
        public void Add(User user)
        {
            context.Users.Add(user);
        }

        public void Delete(User user)
        {
            context.Users.Remove(user);
        }

        public User FindByID(int id, params int[] ids)
        {
            return context.Users.Find(id);
        }

        public List<User> GetAll()
        {
            return context.Users.ToList();
        }

        public User GetByUsernameAndPinCode(User user)
        {
            return context.Users.Single(u => u.Username == user.Username && u.Password == user.Password);
        }

        public User Search(Expression<Func<User, bool>> pred)
        {
            try
            {
                User user = context.Users.Single(pred);
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<User> SearchUsers(Expression<Func<User, bool>> pred)
        {
            return context.Users.Where(pred).ToList();
        }
    }
}
