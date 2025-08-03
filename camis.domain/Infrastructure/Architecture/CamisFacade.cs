using System;
using System.Data.Common;
using intapscamis.camis.data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace intapscamis.camis.domain.Infrastructure.Architecture
{
    public interface ICamisFacade
    {
        void PassContext(ICamisService service, CamisContext context);
    }

    public abstract class CamisFacade : ICamisFacade
    {
        public CamisFacade()
        {

        }

        public virtual void PassContext(ICamisService service, CamisContext context)
        {
            service.SetContext(context);
        }

       
        public virtual TReturn Transact<TReturn>(CamisContext context, Func<IDbContextTransaction, TReturn> func)
        {
            var transaction = context.Database.BeginTransaction();
            try
            {
                var ret = func.Invoke(transaction);
                transaction.Commit();
                return ret;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public virtual void Transact(CamisContext context, Action<IDbContextTransaction> func)
        {
            var transaction = context.Database.BeginTransaction();
            try
            {
                func.Invoke(transaction);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}