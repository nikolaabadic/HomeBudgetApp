using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgetApp.Data
{
    public interface IRepository<T>
    {
        void Add(T param);
        List<T> GetAll();
        T FindByID(int id, params int[] ids);
        void Delete(T param);
    }
}
