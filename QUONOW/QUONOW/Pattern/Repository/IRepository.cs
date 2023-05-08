using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QUONOW.Pattern.Repository
{
    public interface IRepository<T> where T : class, new()
    {
        void Save(T t);

        //T Get(Func<T, bool> prdicate);
        void Update(T t);

        void Delete(T t);

        T Select(object Id);

        List<T> SelectAll();

    }
}