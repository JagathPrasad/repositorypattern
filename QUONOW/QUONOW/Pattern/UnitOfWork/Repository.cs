using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QUONOW.Models;
using System.Data.Entity;

namespace QUONOW.Pattern.Repository
{
    public class Repository<T> : IRepository<T> where T : class, new()
    {
        QUONOWEntities db ;
        public Repository(QUONOWEntities contex)
        {
            this.db = contex;
        }
       
        public void Save(T t)
        {
            this.db.Set<T>().Add(t);
        }

        public void Update(T t)
        {
            this.db.Set<T>().Attach(t);
            this.db.Entry(t).State = System.Data.Entity.EntityState.Modified;
        }

        public void Delete(T t)
        {
            this.db.Set<T>().Attach(t);
            this.db.Entry(t).State = System.Data.Entity.EntityState.Modified;
        }

        public T Select(object Id)
        {
            return this.db.Set<T>().Find(Id);
        }


        public List<T> SelectAll()
        {
            return this.db.Set<T>().ToList();
        }

    }
}