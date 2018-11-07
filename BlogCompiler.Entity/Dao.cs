using System;
using System.Transactions;
using System.Collections.Generic;

namespace BlogCompiler.Entity
{
    public abstract class Dao<R> : IDao
    {
        protected IList<T> Transaction<T>(Func<BlogCompilerContext, IList<T>> action)
        {
            using (var context = new BlogCompilerContext())
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        var ret = action(context);
                        transaction.Complete();
                        return ret;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
        }
        protected T Transaction<T>(Func<BlogCompilerContext, T> action)
        {
            using (var context = new BlogCompilerContext())
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        var ret = action(context);
                        transaction.Complete();
                        return ret;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
        }
        protected Object Transaction(Func<BlogCompilerContext, Object> action)
        {
            using (var context = new BlogCompilerContext())
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        var ret = action(context);
                        transaction.Complete();
                        return ret;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
        }
        protected int Transaction(Func<BlogCompilerContext, int> action)
        {
            using (var context = new BlogCompilerContext())
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        var ret = action(context);
                        transaction.Complete();
                        return ret;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
        }
        protected void Transaction(Action<BlogCompilerContext> action)
        {
            using (var context = new BlogCompilerContext())
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        action(context);
                        transaction.Complete();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
        }
    }
}
