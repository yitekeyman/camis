using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace intapscamis.camis.data
{
    public interface IUnitOfWork : IDisposable
    {
        bool Save();

        IRepository<T> Repository<T>() where T : class;
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private readonly Dictionary<string, object> _repositories = new Dictionary<string, object>();
        private bool disposed;

        public UnitOfWork()
        {
            // _context = new DbContext();
        }

        public UnitOfWork(DbContext context)
        {
            _context = context;
        }

        public bool Save()
        {
            try
            {
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);

            if (_repositories.ContainsKey(type.Name)) return (Repository<T>) _repositories[type.Name];

            var repoInstance = Activator.CreateInstance(type.MakeGenericType(type), _context);

            _repositories.Add(type.Name, repoInstance);

            return (Repository<T>) repoInstance;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
                if (disposing)
                    _context.Dispose();
            disposed = true;
        }
    }
}