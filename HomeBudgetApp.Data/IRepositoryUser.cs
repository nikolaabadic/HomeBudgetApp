using HomeBudgetApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HomeBudgetApp.Data
{
    public interface IRepositoryUser : IRepository<User>
    {
        public User GetByUsernameAndPinCode(User user);
        public User Search(Expression<Func<User, bool>> pred);
        public List<User> SearchUsers(Expression<Func<User, bool>> pred);
    }
}
