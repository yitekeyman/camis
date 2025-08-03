using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace intapscamis.camis.data
{
    public interface IRepository<T> where T : class
    {
        long Count(Expression<Func<T, bool>> predicate);

        bool Delete(T entity);

        bool Delete(IEnumerable<T> entities);

        bool Delete(Expression<Func<T, bool>> predicate);

        bool DeleteById(long id);

        void Edit(T entity);

        bool Exists(Expression<Func<T, bool>> predicate);

        IEnumerable<T> GetAll();

        T GetById(long id);

        bool Insert(T entity);

        bool Insert(IEnumerable<T> entities);

//        bool InsertOrUpdate(IEnumerable<T> entities);

        bool InsertOrUpdate(T entity);

        T SelectSingle(Expression<Func<T, bool>> predicate);

        IEnumerable<T> Select(Expression<Func<T, bool>> predicate);

        IEnumerable<T> Search(Expression<Func<T, bool>> predicate, int top);

        void Detach(T entity);
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        public readonly DbContext CamisContext;
        public DbSet<T> DbSet;

        public Repository(DbContext context)
        {
            CamisContext = context;
            DbSet = context.Set<T>();
        }

        public long Count(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Count(predicate);
        }

        public bool Delete(T entity)
        {
            if (entity == null)
                return false;
            if (CamisContext.Entry(entity).State == EntityState.Detached) CamisContext.Attach(entity);

            CamisContext.Remove(entity);
            return true;
        }

        public bool Delete(IEnumerable<T> entities)
        {
            return entities.All(Delete);
        }

        public bool Delete(Expression<Func<T, bool>> predicate)
        {
            return Delete(DbSet.Where(predicate).ToList());
        }

        public bool DeleteById(long id)
        {
            return Delete(GetById(id));
        }

        public void Edit(T entity)
        {
            CamisContext.Entry(entity).State = EntityState.Modified;
        }

        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Any(predicate);
        }

        public IEnumerable<T> GetAll()
        {
            return DbSet.ToList();
        }

        public T GetById(long id)
        {
            return DbSet.Find(id);
        }

        public bool Insert(T entity)
        {
            try
            {
                if (entity == null) return false;

                DbSet.Add(entity);
                return true;
            }
            catch (DbException)
            {
                return false;
            }
        }

        public bool Insert(IEnumerable<T> entities)
        {
            return entities.All(Insert);
        }

        public bool InsertOrUpdate(T entity)
        {
            try
            {
                if (entity == null) throw new ArgumentNullException(nameof(entity));
                DbSet.Add(entity);

                return true;
            }
            catch (DbException)
            {
                return false;
            }
        }

        public T SelectSingle(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate).ToList().FirstOrDefault();
        }

        public IEnumerable<T> Select(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public IEnumerable<T> Search(Expression<Func<T, bool>> predicate, int top = 0)
        {
            return top == 0 ? DbSet.Where(predicate).ToList() : DbSet.Where(predicate).ToList().Take(top);
        }

        public void Detach(T entity)
        {
            CamisContext.Entry(entity).State = EntityState.Detached;
        }
    }
}