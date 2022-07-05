using Microsoft.EntityFrameworkCore;

namespace Infoware.UnitOfWork
{
    public interface IRepository<TContext, TEntity> : IRepository<TEntity>
        where TContext : DbContext
        where TEntity : class
    {
    }
}